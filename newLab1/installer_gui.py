import tkinter as tk
from tkinter import ttk, messagebox, filedialog
import os
import uuid
import hashlib
from cryptography.fernet import Fernet
import sys
import threading

# --- Функции для работы с лицензией ---
def get_mac_address():
    """Получает MAC-адрес системы."""
    mac_num = uuid.getnode()
    mac = ':'.join(('%012X' % mac_num)[i:i+2] for i in range(0, 12, 2))
    return mac

def generate_encryption_key():
    """Генерирует ключ шифрования Fernet."""
    return Fernet.generate_key()

def encrypt_license(mac_address, key):
    """Хеширует MAC-адрес (SHA-256) и шифрует хеш."""
    mac_hash = hashlib.sha256(mac_address.encode()).hexdigest()
    fernet = Fernet(key)
    encrypted_license = fernet.encrypt(mac_hash.encode())
    return encrypted_license

def find_usb_drives():
    """Ищет подключенные съемные USB-накопители (Windows)."""
    drives = []
    for drive_letter in "ABCDEFGHIJKLMNOPQRSTUVWXYZ":
        path = f"{drive_letter}:\\"
        try:
            if os.path.exists(path) and os.path.ismount(path):
                 try:
                     import win32api, win32file
                     drive_type = win32file.GetDriveType(path)
                     if drive_type == win32file.DRIVE_REMOVABLE:
                         drives.append(path)
                 except ImportError:
                     print("Предупреждение: библиотека pywin32 не найдена. Проверка типа диска пропущена.")
                     drives.append(path)
                 except Exception as e:
                     print(f"Не удалось определить тип диска {path}: {e}")
                     if path not in drives: drives.append(path)
        except OSError:
            continue
    return drives

# --- Функции GUI ---
def update_status(message):
    """Обновляет текстовое поле статуса в главном потоке."""
    status_text.config(state=tk.NORMAL)
    status_text.insert(tk.END, message + "\n")
    status_text.see(tk.END)
    status_text.config(state=tk.DISABLED)
    root.update_idletasks()

def perform_installation(selected_drive):
    """Выполняет процесс установки лицензии в отдельном потоке."""
    # ... (код этой функции остается без изменений) ...
    try:
        update_status(f"Начало установки на диск: {selected_drive}")

        mac_address = get_mac_address()
        update_status(f"MAC-адрес текущего компьютера: {mac_address}")

        encryption_key = generate_encryption_key()
        update_status("Ключ шифрования сгенерирован.")

        encrypted_license = encrypt_license(mac_address, encryption_key)
        update_status("Данные лицензии зашифрованы.")

        key_file_path = os.path.join(selected_drive, 'encryption.key')
        license_file_path = os.path.join(selected_drive, 'license.lic')

        update_status(f"Сохранение ключа шифрования: {key_file_path}")
        with open(key_file_path, 'wb') as file:
            file.write(encryption_key)

        update_status(f"Сохранение файла лицензии: {license_file_path}")
        with open(license_file_path, 'wb') as file:
            file.write(encrypted_license)

        update_status("-" * 20)
        update_status("Установка успешно завершена!")
        update_status(f"Файлы 'encryption.key' и 'license.lic' сохранены на {selected_drive}")
        messagebox.showinfo("Успех", f"Установка лицензии на диск {selected_drive} завершена.")

    except IOError as e:
        error_msg = f"Ошибка записи на диск {selected_drive}: {e}\nПроверьте права доступа и свободное место."
        update_status(f"\nОШИБКА: {error_msg}")
        messagebox.showerror("Ошибка установки", error_msg)
    except Exception as e:
        error_msg = f"Произошла непредвиденная ошибка во время установки: {e}"
        update_status(f"\nОШИБКА: {error_msg}")
        messagebox.showerror("Ошибка установки", error_msg)
    finally:
        # Всегда включаем кнопки обратно после завершения
        set_buttons_state(tk.NORMAL)
        drive_combo.config(state="readonly")

def perform_deletion(selected_drive):
    """Выполняет процесс удаления файлов лицензии в отдельном потоке."""
    key_deleted = False
    lic_deleted = False
    key_error = None
    lic_error = None
    key_exists = False
    lic_exists = False

    try:
        update_status(f"Начало удаления лицензии с диска: {selected_drive}")

        key_file_path = os.path.join(selected_drive, 'encryption.key')
        license_file_path = os.path.join(selected_drive, 'license.lic')

        # --- Удаление ключа ---
        update_status(f"Проверка файла ключа: {key_file_path}")
        if os.path.exists(key_file_path):
            key_exists = True
            update_status("Файл ключа найден. Попытка удаления...")
            try:
                os.remove(key_file_path)
                key_deleted = True
                update_status("Файл ключа успешно удален.")
            except OSError as e:
                key_error = f"Не удалось удалить файл ключа: {e}"
                update_status(f"ОШИБКА: {key_error}")
        else:
            update_status("Файл ключа не найден (удалять нечего).")

        # --- Удаление лицензии ---
        update_status(f"Проверка файла лицензии: {license_file_path}")
        if os.path.exists(license_file_path):
            lic_exists = True
            update_status("Файл лицензии найден. Попытка удаления...")
            try:
                os.remove(license_file_path)
                lic_deleted = True
                update_status("Файл лицензии успешно удален.")
            except OSError as e:
                lic_error = f"Не удалось удалить файл лицензии: {e}"
                update_status(f"ОШИБКА: {lic_error}")
        else:
            update_status("Файл лицензии не найден (удалять нечего).")

        update_status("-" * 20)
        # --- Итог ---
        if not key_exists and not lic_exists:
             update_status("Файлы лицензии не найдены на диске.")
             messagebox.showinfo("Удаление", f"Файлы лицензии не найдены на диске {selected_drive}.")
        elif key_error or lic_error:
            errors = "\n".join(filter(None, [key_error, lic_error]))
            update_status(f"Удаление завершено с ошибками:\n{errors}")
            messagebox.showerror("Ошибка удаления", f"Не удалось удалить один или оба файла лицензии с диска {selected_drive}:\n{errors}")
        elif key_deleted or lic_deleted:
             update_status("Удаление лицензии успешно завершено.")
             messagebox.showinfo("Успех", f"Файлы лицензии успешно удалены с диска {selected_drive}.")
        else: # Файлы были, но не удалились (хотя ошибок не было - странный случай)
            update_status("Не удалось удалить найденные файлы лицензии (причина неизвестна).")
            messagebox.showwarning("Удаление", f"Не удалось удалить найденные файлы лицензии с диска {selected_drive}.")


    except Exception as e:
        error_msg = f"Произошла непредвиденная ошибка во время удаления: {e}"
        update_status(f"\nКРИТИЧЕСКАЯ ОШИБКА: {error_msg}")
        messagebox.showerror("Ошибка удаления", error_msg)
    finally:
        # Всегда включаем кнопки обратно после завершения
        set_buttons_state(tk.NORMAL)
        drive_combo.config(state="readonly")

def start_task_thread(task_function):
    """Общая функция для запуска задачи (установка/удаление) в потоке."""
    selected_drive = drive_combo.get()
    if not selected_drive:
        messagebox.showwarning("Диск не выбран", "Пожалуйста, выберите USB-накопитель из списка.")
        return
    if not os.path.isdir(selected_drive):
         messagebox.showerror("Неверный путь", f"Выбранный путь '{selected_drive}' не является допустимой папкой.")
         return

    # Отключаем кнопки
    set_buttons_state(tk.DISABLED)
    drive_combo.config(state=tk.DISABLED)

    # Очищаем лог
    status_text.config(state=tk.NORMAL)
    status_text.delete('1.0', tk.END)
    status_text.config(state=tk.DISABLED)

    # Запускаем поток
    task_thread = threading.Thread(target=task_function, args=(selected_drive,), daemon=True)
    task_thread.start()

def set_buttons_state(state):
    """Включает или выключает кнопки действий."""
    install_button.config(state=state)
    delete_button.config(state=state)
    refresh_button.config(state=state)


def refresh_drives():
    """Обновляет список доступных USB-дисков."""
    update_status("Поиск USB-накопителей...")
    drives = find_usb_drives()
    drive_combo['values'] = drives
    if drives:
        drive_combo.current(0)
        drive_combo.config(state="readonly")
        update_status(f"Найдены диски: {', '.join(drives)}. Выберите нужный.")
        set_buttons_state(tk.NORMAL) # Включить кнопки
    else:
        drive_combo.set('')
        drive_combo.config(state=tk.DISABLED)
        update_status("Подходящие USB-накопители не найдены.")
        messagebox.showwarning("Диски не найдены", "Съемные USB-накопители не найдены.\nПодключите накопитель и нажмите 'Обновить'.")
        set_buttons_state(tk.DISABLED) # Оставить кнопки выключенными

# --- Настройка GUI ---
root = tk.Tk()
root.title("Установщик лицензии программы")
root.geometry("550x450") # Немного увеличим высоту для новой кнопки

style = ttk.Style()
style.theme_use('clam')

# --- Фрейм для выбора диска ---
drive_frame = ttk.Frame(root, padding="10 10 10 5")
drive_frame.pack(fill=tk.X)

drive_label = ttk.Label(drive_frame, text="Выберите USB-накопитель:")
drive_label.pack(side=tk.LEFT, padx=(0, 5))

drive_combo = ttk.Combobox(drive_frame, state="disabled", width=40)
drive_combo.pack(side=tk.LEFT, fill=tk.X, expand=True)

refresh_button = ttk.Button(drive_frame, text="Обновить", command=refresh_drives, state=tk.NORMAL) # Кнопка Обновить всегда активна
refresh_button.pack(side=tk.LEFT, padx=(10, 0))

# --- Фрейм для кнопок действий ---
button_frame = ttk.Frame(root, padding="10 5 10 10")
button_frame.pack(fill=tk.X)

# Кнопка Установки
install_button = ttk.Button(button_frame, text="Установить лицензию на выбранный диск",
                             command=lambda: start_task_thread(perform_installation), state=tk.DISABLED)
install_button.pack(pady=(0, 5)) # Отступ снизу

# Кнопка Удаления
delete_button = ttk.Button(button_frame, text="Удалить лицензию с выбранного диска",
                            command=lambda: start_task_thread(perform_deletion), state=tk.DISABLED)
delete_button.pack(pady=(5, 0)) # Отступ сверху

# --- Область статуса / Лог ---
status_frame = ttk.LabelFrame(root, text="Журнал состояния", padding="10")
status_frame.pack(fill=tk.BOTH, expand=True, padx=10, pady=(0, 10))

status_text = tk.Text(status_frame, wrap=tk.WORD, state=tk.DISABLED, height=12, font=('Segoe UI', 9))
scrollbar = ttk.Scrollbar(status_frame, orient=tk.VERTICAL, command=status_text.yview)
status_text['yscrollcommand'] = scrollbar.set
scrollbar.pack(side=tk.RIGHT, fill=tk.Y)
status_text.pack(side=tk.LEFT, fill=tk.BOTH, expand=True)

# --- Инициализация ---
root.after(150, refresh_drives) # Запуск поиска дисков при старте
set_buttons_state(tk.DISABLED) # Убедимся, что кнопки выключены до нахождения дисков

root.mainloop()