## Запуск проекта

Запустить Docker, в консоли в папке проекта выполнить команду

```sh
docker-compose up -d
```

## Описание задания

См. файл `Тестовое задание .Net 2.pdf`

## Описание API

### UserApi:

- Регистация:

  > http://localhost:8080/api/user/register `POST`
  - Модель запроса:

  ```cs
  {
      string Name
      string Password
  }
  ```

  - Возвращаемая модель:

  ```ts
  {
      access_token: string
      expires_in: number
      refresh_expires_in: number
      refresh_token: string
      token_type: string
      id_token: string
      "not-before-policy": number
      session_state: string
      scope: string
  }
  ```

- Вход:

  > http://localhost:8080/api/user/login `POST`
  - Модель запроса:

  ```cs
  {
      string Name
      string Password
  }
  ```

  - Возвращаемая модель:

  ```ts
  {
      access_token: string
      expires_in: number
      refresh_expires_in: number
      refresh_token: string
      token_type: string
      id_token: string
      "not-before-policy": number
      session_state: string
      scope: string
  }
  ```

- Выход (logout):

  > http://localhost:8080/api/user/logout `POST`
  - Модель запроса:

  ```cs
  {
      string RefreshToken
  }
  ```

  - Возврат: `200 (OK)`

### CurrencyApi:

- Получение списка отслеживаемых валют:

  > http://localhost:8080/api/currency/favorites `GET`

  > Требует авторизацию
  - Возвращаемая модель:

  ```ts
  {
    Data: Array<{
      Id: string;
      Name: string;
      Rate: number;
    }>;
  }
  ```
