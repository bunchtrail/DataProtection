import os
import uuid
import hashlib
from cryptography.fernet import Fernet
import sys # Для получения пути к скрипту

def get_mac_address():
    """Получает MAC-адрес системы."""
    mac_num = uuid.getnode()
    mac = ':'.join(('%012X' % mac_num)[i:i+2] for i in range(0, 12, 2))
    return mac

def generate_encryption_key():
    """Генерирует безопасный ключ для шифрования Fernet."""
    return Fernet.generate_key()

def encrypt_license(mac_address, key):
    """Хеширует MAC-адрес и шифрует хеш с помощью ключа."""
    # 1. Хешируем MAC-адрес с использованием SHA-256
    mac_hash = hashlib.sha256(mac_address.encode()).hexdigest()

    # 2. Шифруем полученный хеш
    fernet = Fernet(key)
    encrypted_license = fernet.encrypt(mac_hash.encode()) # Шифруем байтовое представление хеша
    return encrypted_license

def get_usb_drive():
    """
    Ищет первый подключенный сменный USB-накопитель (флешку)
    в Windows. Возвращает путь к корню диска (например, 'E:\\').
    """
    for drive in "DEFGHIJKLMNOPQRSTUVWXYZ":
        path = f"{drive}:\\"
        try:
            if os.path.exists(path) and os.path.ismount(path):
                 return path
        except OSError:
            continue
    return None

def main():
    """Основная функция установщика."""
    print("Starting license installer...")

    # 1. Находим USB-накопитель
    usb_path = get_usb_drive()
    if not usb_path:
        print("\nError: USB drive not found.")
        print("Please insert a USB drive and run the installer again.")
        input("Press Enter to exit.")
        return # Выход, если флешка не найдена

    print(f"USB drive found at: {usb_path}")

    try:
        # 2. Получаем MAC-адрес текущей машины
        mac_address = get_mac_address()
        print(f"Current machine MAC address: {mac_address}")

        # 3. Генерируем ключ шифрования
        encryption_key = generate_encryption_key()
        print("Generated encryption key.")

        # 4. Шифруем хеш MAC-адреса
        encrypted_license = encrypt_license(mac_address, encryption_key)
        print("Encrypted license data created.")

        # 5. Определяем пути для сохранения файлов
        key_file_path = os.path.join(usb_path, 'encryption.key')
        license_file_path = os.path.join(usb_path, 'license.lic')

        # 6. Сохраняем ключ шифрования на флешку (в бинарном режиме)
        print(f"Saving encryption key to: {key_file_path}")
        with open(key_file_path, 'wb') as file:
            file.write(encryption_key)

        # 7. Сохраняем зашифрованную лицензию на флешку (в бинарном режиме)
        print(f"Saving license file to: {license_file_path}")
        with open(license_file_path, 'wb') as file:
            file.write(encrypted_license)

        print("\nInstallation completed successfully.")
        print(f"License files ('encryption.key' and 'license.lic') saved to {usb_path}")

        # 8. Удаление самого установщика (опционально и потенциально рискованно)
        try:
            # Получаем полный путь к текущему исполняемому скрипту
            if getattr(sys, 'frozen', False):
                # Если запущено как .exe (например, через PyInstaller)
                installer_path = sys.executable
            else:
                # Если запущено как .py скрипт
                installer_path = os.path.abspath(__file__)

            print(f"\nAttempting to remove installer: {installer_path}")
            # Даем небольшую паузу перед удалением
            # import time
            # time.sleep(1)
            os.remove(installer_path)
            print("Installer removed successfully.")
        except OSError as e:
            print(f"\nWarning: Could not remove the installer file.")
            print(f"Error details: {e}")
            print("You may need to delete it manually.")
        except Exception as e:
            print(f"\nWarning: An unexpected error occurred while trying to remove the installer.")
            print(f"Error details: {e}")

    except IOError as e:
        print(f"\nError: Could not write files to USB drive {usb_path}.")
        print(f"Details: {e}")
        print("Check drive permissions and available space.")
    except Exception as e:
        # Ловим другие возможные ошибки
        print(f"\nAn unexpected error occurred during installation: {str(e)}")

    input("\nPress Enter to exit.")

if __name__ == "__main__":
    main()