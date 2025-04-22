import tkinter as tk
from tkinter import ttk, messagebox
import os
import uuid
import hashlib
from cryptography.fernet import Fernet, InvalidToken
import threading

# --- Функции из оригинального кода (немного изменены для работы с путем) ---
def get_mac_address():
    """Получает MAC-адрес системы."""
    mac_num = uuid.getnode()
    mac = ':'.join(('%012X' % mac_num)[i:i+2] for i in range(0, 12, 2))
    return mac

def find_usb_drives():
    """
    Ищет все подключенные сменные USB-накопители (флешки) в Windows.
    Возвращает список путей к корню диска (например, ['E:\\', 'F:\\']).
    """
    drives = []
    for drive_letter in "ABCDEFGHIJKLMNOPQRSTUVWXYZ":
        path = f"{drive_letter}:\\"
        try:
            if os.path.exists(path) and os.path.ismount(path):
                 try:
                     import win32api
                     import win32file
                     drive_type = win32file.GetDriveType(path)
                     if drive_type == win32file.DRIVE_REMOVABLE:
                         drives.append(path)
                 except ImportError:
                     print("Warning: pywin32 not found. Drive type check skipped.")
                     drives.append(path)
                 except Exception:
                      if path not in drives:
                         drives.append(path)
        except OSError:
            continue
    return drives

def read_file_from_drive(drive_path, file_name):
    """
    Читает указанный файл с указанного диска.
    Вызывает исключение FileNotFoundError или IOError.
    """
    file_path = os.path.join(drive_path, file_name)
    if not os.path.exists(file_path):
        raise FileNotFoundError(f"File '{file_name}' not found on drive {drive_path}.")

    try:
        with open(file_path, 'rb') as file:
            return file.read()
    except IOError as e:
        raise IOError(f"Could not read file '{file_name}' from {drive_path}: {e}")

def decrypt_license(encrypted_license, encryption_key):
    """Расшифровывает данные лицензии с использованием ключа."""
    try:
        fernet = Fernet(encryption_key)
        decrypted_data = fernet.decrypt(encrypted_license)
        return decrypted_data.decode()
    except InvalidToken:
        raise ValueError("Decryption failed: Invalid token (wrong key or corrupted data).")
    except Exception as e:
        raise ValueError(f"Decryption failed: {e}")

# --- Функции для GUI ---

def update_status(message, is_error=False, is_success=False):
    """Обновляет метку статуса в главном потоке."""
    status_label.config(text=message)
    if is_error:
        status_label.config(foreground="red")
    elif is_success:
        status_label.config(foreground="green")
    else:
        status_label.config(foreground="black")
    root.update_idletasks()

def run_main_program_logic():
    """Сюда помещается основная логика вашей программы ПОСЛЕ успешной проверки."""
    update_status("License Verified! Running main program...", is_success=True)
    messagebox.showinfo("Success", "License verified successfully!\n(Main program logic would run here)")
    # Пример: можно открыть новое окно, разблокировать кнопки и т.д.
    # root.destroy() # Можно закрыть окно проверки
    # launch_actual_app()

def perform_verification(selected_drive):
    """Выполняет проверку лицензии в отдельном потоке."""
    try:
        update_status(f"Verifying license on drive: {selected_drive}")

        # 1. Читаем ключ шифрования
        update_status("Reading encryption key...")
        encryption_key = read_file_from_drive(selected_drive, 'encryption.key')
        update_status("Encryption key found.")

        # 2. Читаем файл лицензии
        update_status("Reading license file...")
        encrypted_license = read_file_from_drive(selected_drive, 'license.lic')
        update_status("License file found.")

        # 3. Получаем текущий MAC
        update_status("Getting current MAC address...")
        current_mac = get_mac_address()
        update_status(f"Current MAC: {current_mac}")

        # 4. Расшифровываем лицензию
        update_status("Decrypting license...")
        stored_mac_hash = decrypt_license(encrypted_license, encryption_key)
        update_status("License decrypted.")

        # 5. Хешируем текущий MAC
        update_status("Hashing current MAC address...")
        current_mac_hash = hashlib.sha256(current_mac.encode()).hexdigest()

        # 6. Сравниваем хеши
        update_status("Comparing license data with current system...")
        if current_mac_hash == stored_mac_hash:
            update_status("Success: License is valid!", is_success=True)
            # Запустить основную логику программы (можно в главном потоке после завершения этого)
            root.after(100, run_main_program_logic)
        else:
            error_msg = "Error: Invalid license (MAC address mismatch)."
            update_status(error_msg, is_error=True)
            messagebox.showerror("Verification Failed", error_msg)

    except FileNotFoundError as e:
        error_msg = f"Error: {e}"
        update_status(error_msg, is_error=True)
        messagebox.showerror("Verification Failed", f"{error_msg}\nPlease ensure the correct USB drive is selected and contains 'encryption.key' and 'license.lic'.")
    except (ValueError, IOError) as e: # Ошибки дешифровки или чтения файла
        error_msg = f"Error: {e}"
        update_status(error_msg, is_error=True)
        messagebox.showerror("Verification Failed", f"{error_msg}\nLicense files might be missing, corrupted, or unreadable.")
    except Exception as e:
        error_msg = f"An unexpected error occurred: {str(e)}"
        update_status(error_msg, is_error=True)
        messagebox.showerror("Verification Failed", error_msg)
    finally:
        # Снова включить кнопки после завершения проверки (если не было успеха)
        if not status_label.cget("foreground") == "green":
             verify_button.config(state=tk.NORMAL)
             refresh_button.config(state=tk.NORMAL)
             drive_combo.config(state="readonly")


def start_verification_thread():
    """Запускает проверку лицензии в отдельном потоке."""
    selected_drive = drive_combo.get()
    if not selected_drive:
        messagebox.showwarning("No Drive Selected", "Please select the USB drive containing the license.")
        return
    if not os.path.isdir(selected_drive):
         messagebox.showerror("Invalid Path", f"The selected path '{selected_drive}' is not a valid directory.")
         return

    # Отключить кнопки на время проверки
    verify_button.config(state=tk.DISABLED)
    refresh_button.config(state=tk.DISABLED)
    drive_combo.config(state=tk.DISABLED)
    update_status("Starting verification...") # Сбросить статус

    # Запустить проверку в потоке
    verify_thread = threading.Thread(target=perform_verification, args=(selected_drive,), daemon=True)
    verify_thread.start()

def refresh_drives_main():
    """Обновляет список доступных USB-дисков для главного окна."""
    update_status("Searching for USB drives...")
    drives = find_usb_drives()
    drive_combo['values'] = drives
    if drives:
        drive_combo.current(0)
        update_status(f"Found drives. Select the one with the license.")
        verify_button.config(state=tk.NORMAL) # Включить кнопку проверки
    else:
        drive_combo.set('')
        update_status("No suitable USB drives found.", is_error=True)
        messagebox.showwarning("No Drives", "No removable USB drives found. Please insert the licensed drive and click Refresh.")
        verify_button.config(state=tk.DISABLED) # Отключить кнопку проверки

# --- Настройка GUI ---
root = tk.Tk()
root.title("Program License Verification")
root.geometry("450x250")

# Фрейм для выбора диска
drive_frame = ttk.Frame(root, padding="10")
drive_frame.pack(fill=tk.X, pady=10)

drive_label = ttk.Label(drive_frame, text="Select Licensed USB Drive:")
drive_label.pack(side=tk.LEFT, padx=(0, 5))

drive_combo = ttk.Combobox(drive_frame, state="readonly", width=35)
drive_combo.pack(side=tk.LEFT, fill=tk.X, expand=True)

refresh_button = ttk.Button(drive_frame, text="Refresh", command=refresh_drives_main)
refresh_button.pack(side=tk.LEFT, padx=(5, 0))

# Кнопка проверки
verify_button = ttk.Button(root, text="Verify License and Run", command=start_verification_thread, state=tk.DISABLED)
verify_button.pack(pady=10)

# Метка статуса
status_label = ttk.Label(root, text="Please select a drive and click Verify.", padding="10", wraplength=400) # wraplength для переноса текста
status_label.pack(fill=tk.X, pady=10)

# Первоначальное сканирование дисков
root.after(100, refresh_drives_main)

root.mainloop()