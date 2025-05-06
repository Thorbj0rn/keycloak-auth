# Пример jwt аутентификации и авторизации в .Net с Keycloak

  1. Разворачиваем в докере keycloak и api.
  2. Заходим в [админ консоль](http://localhost:18080/) keycloak. Логин/пароль: admin/admin.
  3. Manage realms -> Create realm -> Создаём новый realm с именем keycloak-auth-demo 
     ![Создаём realm](https://github.com/Thorbj0rn/keycloak-auth/blob/main/screenshots/1.png)
  4. Заходим в Realm Settings -> Login, включаем возможность регистрации пользователей
     ![Включаем регистрацию](https://github.com/Thorbj0rn/keycloak-auth/blob/main/screenshots/2.png)
  5. Clients -> Create client -> Создаём клиента с именем public-client
     ![Создаём клиента](https://github.com/Thorbj0rn/keycloak-auth/blob/main/screenshots/3.png)
     Добавляем нужные варианты аутентификации(authentication flow)
     ![Добавляем флоу](https://github.com/Thorbj0rn/keycloak-auth/blob/main/screenshots/4.png)
     Задаём редирект урлы
     ![Задаём урлы](https://github.com/Thorbj0rn/keycloak-auth/blob/main/screenshots/5.png)
     Сохраняем клиент.
  6. Clients -> Выбираем public-client -> Создаём в клиенте роль admin.     
  7. После регистрации пользователя, добавляем ему роль public-client:admin
     Users -> {user_name} -> Role mapping -> Assing role 
     ![Создаём realm](https://github.com/Thorbj0rn/keycloak-auth/blob/main/screenshots/6.png)

