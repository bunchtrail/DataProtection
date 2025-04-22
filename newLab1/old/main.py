import os
import uuid
import hashlib
from cryptography.fernet import Fernet

def get_mac_address():
    """Получает MAC-адрес системы."""
    # uuid.getnode() возвращает 48-битное целое число
    # Преобразуем его в стандартный формат MAC-адреса
    mac_num = uuid.getnode()
    mac = ':'.join(('%012X' % mac_num)[i:i+2] for i in range(0, 12, 2))
    # Некоторые реализации uuid.getnode могут возвращать MAC с другим порядком байт,
    # старый метод ниже может работать в некоторых случаях, но менее надежен:
    # mac = ':'.join(['{:02x}'.format((uuid.getnode() >> elements) & 0xff) for elements in range(0, 2*6, 8)][::-1])
    return mac

def get_usb_drive():
    """
    Ищет первый подключенный сменный USB-накопитель (флешку)
    в Windows. Возвращает путь к корню диска (например, 'E:\\').
    """
    # Проверяем буквы дисков от D до Z (A, B обычно для флоппи, C для системы)
    for drive in "DEFGHIJKLMNOPQRSTUVWXYZ":
        path = f"{drive}:\\"
        try:
            # os.path.exists проверяет существование пути
            # os.path.ismount проверяет, является ли путь точкой монтирования (для USB/CD/DVD)
            # Дополнительно можно проверить тип диска, но os.path.ismount часто достаточно
            if os.path.exists(path) and os.path.ismount(path):
                 # Простая проверка на наличие файлов может быть не надежна,
                 # но для примера можно добавить:
                 # if os.listdir(path): # Если папка не пуста
                 return path
        except OSError:
            # Может возникнуть ошибка доступа, если диск недоступен
            continue
    return None

def read_file_from_usb(file_name):
    """
    Читает указанный файл с найденного USB-накопителя.
    Вызывает исключение FileNotFoundError, если USB не найден или файл отсутствует.
    """
    usb_path = get_usb_drive()
    if not usb_path:
        raise FileNotFoundError("USB drive not found.")

    file_path = os.path.join(usb_path, file_name)
    if not os.path.exists(file_path):
        raise FileNotFoundError(f"File '{file_name}' not found on USB drive ({usb_path}).")

    try:
        with open(file_path, 'rb') as file: # Открываем в бинарном режиме
            return file.read()
    except IOError as e:
        raise IOError(f"Could not read file '{file_name}' from USB: {e}")


def decrypt_license(encrypted_license, encryption_key):
    """Расшифровывает данные лицензии с использованием ключа."""
    try:
        fernet = Fernet(encryption_key)
        decrypted_data = fernet.decrypt(encrypted_license)
        return decrypted_data.decode() # Декодируем байты в строку (предполагая UTF-8)
    except Exception as e:
        # Обработка ошибок дешифрования (неверный ключ, поврежденные данные)
        raise ValueError(f"Failed to decrypt license: {e}")


def main():
    """Основная функция проверки лицензии."""
    try:
        print("Attempting to verify license...")
        # 1. Читаем ключ шифрования с USB
        print("Reading encryption key from USB...")
        encryption_key = read_file_from_usb('encryption.key')
        print("Encryption key found.")

        # 2. Читаем зашифрованный файл лицензии с USB
        print("Reading license file from USB...")
        encrypted_license = read_file_from_usb('license.lic')
        print("License file found.")

        # 3. Получаем MAC-адрес текущей машины
        print("Getting current MAC address...")
        current_mac = get_mac_address()
        print(f"Current MAC: {current_mac}")

        # 4. Расшифровываем лицензию
        print("Decrypting license...")
        stored_mac_hash = decrypt_license(encrypted_license, encryption_key)
        print("License decrypted.")

        # 5. Хешируем текущий MAC-адрес
        print("Hashing current MAC address...")
        current_mac_hash = hashlib.sha256(current_mac.encode()).hexdigest()
        print(f"Current MAC Hash: {current_mac_hash}")
        print(f"Stored MAC Hash:  {stored_mac_hash}")

        # 6. Сравниваем хеш из лицензии с хешем текущего MAC-адреса
        if current_mac_hash == stored_mac_hash:
            print("\nSuccess: License is valid. USB drive detected and MAC address matches.")
            # Здесь можно запустить основную логику защищенной программы
            print("Program can now proceed...")
        else:
            print("\nError: Invalid license. MAC address mismatch.")

    except FileNotFoundError as e:
        print(f"\nError: {e}")
        print("Please ensure the correct USB drive with license files is connected.")
    except ValueError as e: # Ошибки дешифровки
        print(f"\nError: {e}")
        print("License file might be corrupted or the encryption key is incorrect.")
    except Exception as e:
        # Ловим другие возможные ошибки (например, проблемы с правами доступа, ошибки cryptography)
        print(f"\nAn unexpected error occurred: {str(e)}")

if __name__ == "__main__":
    main()
    input("\nPress Enter to exit.") # Пауза, чтобы увидеть вывод в консоли