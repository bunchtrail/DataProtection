import tkinter as tk
from tkinter import ttk, messagebox
import os
import uuid
import hashlib
from cryptography.fernet import Fernet, InvalidToken
import threading

# --- Функции для работы с лицензией ---
def get_mac_address():
    """Получает MAC-адрес системы."""
    mac_num = uuid.getnode()
    mac = ':'.join(('%012X' % mac_num)[i:i+2] for i in range(0, 12, 2))
    return mac

def find_usb_drives():
    """Ищет подключенные съемные USB-накопители (Windows)."""
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
                     print("Предупреждение: библиотека pywin32 не найдена. Проверка типа диска пропущена.")
                     drives.append(path)
                 except Exception as e:
                     print(f"Не удалось определить тип диска {path}: {e}")
                     if path not in drives:
                         drives.append(path)
        except OSError:
            continue
    return drives

def read_file_from_drive(drive_path, file_name):
    """Читает файл с указанного диска."""
    file_path = os.path.join(drive_path, file_name)
    if not os.path.exists(file_path):
        raise FileNotFoundError(f"Файл '{file_name}' не найден на диске {drive_path}.")
    try:
        with open(file_path, 'rb') as file:
            return file.read()
    except IOError as e:
        raise IOError(f"Не удалось прочитать файл '{file_name}' с диска {drive_path}: {e}")

def decrypt_license(encrypted_license, encryption_key):
    """Расшифровывает данные лицензии."""
    try:
        fernet = Fernet(encryption_key)
        decrypted_data = fernet.decrypt(encrypted_license)
        return decrypted_data.decode()
    except InvalidToken:
        raise ValueError("Ошибка расшифровки: Неверный ключ или данные повреждены.")
    except Exception as e:
        raise ValueError(f"Ошибка расшифровки: {e}")

# --- Функции GUI ---
def update_status(message, status_type="info"):
    """Обновляет метку статуса с разным цветом."""
    status_label.config(text=message)
    if status_type == "error":
        status_label.config(foreground="red")
    elif status_type == "success":
        status_label.config(foreground="green")
    else: # info
        status_label.config(foreground="black")
    root.update_idletasks()

def run_main_program_logic():
    """Симуляция запуска основной логики программы."""
    update_status("Лицензия подтверждена! Запуск основной программы...", status_type="success")
    # Вместо messagebox здесь должен быть код вашей основной программы
    messagebox.showinfo("Успех", "Лицензия успешно проверена!\n(Здесь должен запускаться основной функционал программы)")
    # Например, можно закрыть окно проверки и открыть главное окно программы:
    # root.destroy()
    # main_app_window = create_main_app_window()
    # main_app_window.mainloop()

def perform_verification(selected_drive):
    """Выполняет проверку лицензии в отдельном потоке."""
    try:
        update_status(f"Проверка лицензии на диске: {selected_drive}", "info")

        update_status("Чтение ключа шифрования...", "info")
        encryption_key = read_file_from_drive(selected_drive, 'encryption.key')

        update_status("Чтение файла лицензии...", "info")
        encrypted_license = read_file_from_drive(selected_drive, 'license.lic')

        update_status("Получение MAC-адреса системы...", "info")
        current_mac = get_mac_address()

        update_status("Расшифровка лицензии...", "info")
        stored_mac_hash = decrypt_license(encrypted_license, encryption_key)

        update_status("Хеширование MAC-адреса системы...", "info")
        current_mac_hash = hashlib.sha256(current_mac.encode()).hexdigest()

        update_status("Сравнение данных...", "info")
        if current_mac_hash == stored_mac_hash:
            # Используем root.after для вызова основной логики в главном потоке GUI
            root.after(100, run_main_program_logic)
        else:
            error_msg = "Ошибка: Неверная лицензия (MAC-адрес не совпадает)."
            update_status(error_msg, status_type="error")
            messagebox.showerror("Ошибка проверки", error_msg)
            # Включаем кнопки обратно, если проверка не удалась
            verify_button.config(state=tk.NORMAL)
            refresh_button.config(state=tk.NORMAL)
            drive_combo.config(state="readonly")


    except FileNotFoundError as e:
        error_msg = f"Ошибка: {e}"
        update_status(error_msg, status_type="error")
        messagebox.showerror("Ошибка проверки", f"{error_msg}\nУбедитесь, что выбран правильный USB-диск и он содержит файлы 'encryption.key' и 'license.lic'.")
        verify_button.config(state=tk.NORMAL)
        refresh_button.config(state=tk.NORMAL)
        drive_combo.config(state="readonly")
    except (ValueError, IOError) as e: # Ошибки дешифровки, чтения
        error_msg = f"Ошибка: {e}"
        update_status(error_msg, status_type="error")
        messagebox.showerror("Ошибка проверки", f"{error_msg}\nФайлы лицензии могут отсутствовать, быть повреждены или недоступны для чтения.")
        verify_button.config(state=tk.NORMAL)
        refresh_button.config(state=tk.NORMAL)
        drive_combo.config(state="readonly")
    except Exception as e:
        error_msg = f"Произошла непредвиденная ошибка: {str(e)}"
        update_status(error_msg, status_type="error")
        messagebox.showerror("Критическая ошибка", error_msg)
        verify_button.config(state=tk.NORMAL)
        refresh_button.config(state=tk.NORMAL)
        drive_combo.config(state="readonly")


def start_verification_thread():
    """Запускает проверку лицензии в отдельном потоке."""
    selected_drive = drive_combo.get()
    if not selected_drive:
        messagebox.showwarning("Диск не выбран", "Пожалуйста, выберите USB-накопитель с лицензией.")
        return
    if not os.path.isdir(selected_drive):
         messagebox.showerror("Неверный путь", f"Выбранный путь '{selected_drive}' не является допустимой папкой.")
         return

    # Отключаем кнопки на время проверки
    verify_button.config(state=tk.DISABLED)
    refresh_button.config(state=tk.DISABLED)
    drive_combo.config(state=tk.DISABLED)
    update_status("Запуск проверки лицензии...", "info")

    # Запускаем проверку в потоке
    verify_thread = threading.Thread(target=perform_verification, args=(selected_drive,), daemon=True)
    verify_thread.start()

def refresh_drives_main():
    """Обновляет список дисков в главном окне."""
    update_status("Поиск USB-накопителей...", "info")
    drives = find_usb_drives()
    drive_combo['values'] = drives
    if drives:
        drive_combo.current(0)
        drive_combo.config(state="readonly")
        update_status("Найдены диски. Выберите диск с лицензией и нажмите 'Проверить'.", "info")
        verify_button.config(state=tk.NORMAL)
    else:
        drive_combo.set('')
        drive_combo.config(state=tk.DISABLED)
        update_status("Подходящие USB-накопители не найдены.", status_type="error")
        messagebox.showwarning("Диски не найдены", "Съемные USB-накопители не найдены.\nПодключите диск с лицензией и нажмите 'Обновить'.")
        verify_button.config(state=tk.DISABLED)

# --- Настройка GUI ---
root = tk.Tk()
root.title("Проверка лицензии программы")
root.geometry("500x280") # Уменьшим высоту

# Стиль ttk
style = ttk.Style()
style.theme_use('clam')

# --- Фрейм для выбора диска ---
drive_frame = ttk.Frame(root, padding="10 10 10 5")
drive_frame.pack(fill=tk.X)

drive_label = ttk.Label(drive_frame, text="USB-накопитель с лицензией:")
drive_label.pack(side=tk.LEFT, padx=(0, 5))

drive_combo = ttk.Combobox(drive_frame, state="disabled", width=35)
drive_combo.pack(side=tk.LEFT, fill=tk.X, expand=True)

refresh_button = ttk.Button(drive_frame, text="Обновить", command=refresh_drives_main)
refresh_button.pack(side=tk.LEFT, padx=(10, 0))

# --- Кнопка Проверки ---
button_frame = ttk.Frame(root, padding="10 5 10 10")
button_frame.pack(fill=tk.X)
verify_button = ttk.Button(button_frame, text="Проверить лицензию и запустить", command=start_verification_thread, state=tk.DISABLED)
verify_button.pack()

# --- Метка статуса ---
status_frame = ttk.Frame(root, padding="10 5 10 10")
status_frame.pack(fill=tk.BOTH, expand=True)

status_label = ttk.Label(status_frame, text="Подключите USB-накопитель и нажмите 'Обновить'.",
                         padding="5", wraplength=480, anchor=tk.CENTER, justify=tk.CENTER,
                         font=('Segoe UI', 10)) # Перенос текста, выравнивание
status_label.pack(fill=tk.BOTH, expand=True)

# --- Запуск поиска дисков при старте ---
root.after(150, refresh_drives_main)

root.mainloop()