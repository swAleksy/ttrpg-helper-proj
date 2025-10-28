## Uruchomienie Projektu

### 1. Wymagane narzędzia
Aby rozpocząć pracę z projektem, potrzebne są następujące elementy:
1.  **Docker Desktop** (do zarządzania kontenerami).
2.  **PgAdmin** (lub inne narzędzie do zarządzania bazą danych PostgreSQL).
3.  **Środowisko programistyczne** (np. VS Code, Node.js, itp.) do edycji kodu po stronie, za którą jesteś odpowiedzialny/a (front/back).

***

### 2. Uruchomienie Projektu
Wgłównym folderze pobranego repo 
1.  **Budowanie Obrazów Dockerowych:**
    ```bash
    docker compose build
    ```
    *(Ta komenda buduje **trzy obrazy**: `frontend-app`, `backend-api` i `postgres`.)*
2.  **Uruchomienie Kontenerów:**
    ```bash
    docker compose up -d
    ```
***

### 3. Konfiguracja Podglądu Bazy Danych (PgAdmin)
Aby połączyć się z bazą danych działającą w kontenerze:

1.  Otwórz **PgAdmin**.
2.  Kliknij **PPM** (Prawy Przycisk Myszy) na **Servers** i wybierz **Register** / **Server**.
3.  W zakładce **General** podaj dowolną **Nazwę**.
4.  W zakładce **Connection** ustaw parametry połączenia:
    * **Host name/address:** `localhost`
    * **Port:** `5433` (Ten port jest **zewnętrznym** portem, na którym nasłuchuje Docker (conf w pliku `docker-compose.yml`)).
    * **Maintenance database:** `postgres`
    * **Username:** `postgres`
    * **Password:** `inz12345`

***

### 4. Dodatkowe Informacje i Workflow

#### A. Stosowanie Zmian (Przeładowanie Kontenera)
Aby zastosować zmiany w kodzie i przebudować kontener:

1.  **Wyłączenie Kontenera/Kontenerów:**
    * **Wszystkie kontenery:** `docker compose down`
    * **Konkretny kontener** (np. sam backend): `docker compose down backend-api`
    *(Dostępne nazwy serwisów: `backend-api`, `postgres`, `frontend-app`)*
2.  **Ponowne Budowanie Obrazu** (jeśli zmieniłeś/aś Dockerfile lub zależności):
    ```bash
    docker compose build [NAZWA_SERWISU]
    ```
3.  **Ponowne Uruchomienie:**
    ```bash
    docker compose up -d [NAZWA_SERWISU]
    ```

#### B. Dokumentacja API
Po pomyślnym uruchomieniu kontenera **backend-api**, pełną dokumentację do endpointów (Scalar) znajdziesz pod adresem:
http://localhost:8080/scalar/v1
