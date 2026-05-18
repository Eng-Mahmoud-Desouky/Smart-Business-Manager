# Implementation Plan - Standard Email & Password Authentication

We are implementing standard Email and Password authentication (Login and Registration) for the **Smart Business Manager** cross-platform mobile application using .NET MAUI and Supabase.

This implementation strictly respects:
1. **Manual MVVM architecture**: No source generators or CommunityToolkit packages. Inherits from `BaseViewModel` and uses explicit backing fields with `OnPropertyChanged()` triggers.
2. **Raw HttpClient communications**: Interfacing with Supabase Auth (GoTrue) and REST API endpoints without using the Supabase C# SDK.
3. **Strict Visual Fidelity**: Recreating the visual styling and layout from `authentication.html` using style tokens from `Colors.xaml` and `Styles.xaml`.
4. **Basic/Clean Scope**: Only Email and Password fields. No social logins, and no "Forgot Password" function.

---

## User Review Required

> [!IMPORTANT]
> **File Placement Context**
> The user request mentions creating files directly in `Features/Authentication/`. However, the repository already contains empty placeholder files structured according to the **Single Source of Truth Blueprint** under `Features/Authentication/Views/` and `Features/Authentication/ViewModels/`.
> We will update these existing placeholder files in their current locations to keep the application building successfully and maintain consistency with the other modules (e.g. Clients, Finance, AI).
>
> We will also update `IAuthService` and `AuthService` in `Core/Services/`.

> [!NOTE]
> **Full Name Auto-Generation on Register**
> The `IAuthService.SignUpAsync` interface includes a `string fullName` parameter, and the `profiles` table has a `NOT NULL` constraint on `full_name`. Since the Team Lead explicitly restricted the UI to standard Email and Password fields, we will auto-generate the `fullName` in the `RegisterViewModel` from the Email prefix (e.g., using `email.Split('@')[0]`) to ensure the profile is successfully created in the Supabase database.

---

## Proposed Changes

### 1. Core Services Layer

#### [MODIFY] [IAuthService.cs](file:///c:/Users/Hp/Smart-Business-Manager/SmartBusinessManager/Core/Services/IAuthService.cs)
Keep the existing interface matching the blueprint:
- `SignInAsync(string email, string password)`
- `SignUpAsync(string email, string password, string fullName)`
- `SignOutAsync()`

#### [MODIFY] [AuthService.cs](file:///c:/Users/Hp/Smart-Business-Manager/SmartBusinessManager/Core/Services/AuthService.cs)
Implement the authentication services using `HttpClient`:
- **Login (`SignInAsync`)**:
  - Send HTTP POST to `${Constants.AuthBase}/token?grant_type=password` with headers `apikey: Constants.SupabaseAnonKey` and `Content-Type: application/json`.
  - Extract the session tokens (`access_token`, `refresh_token`) and user `id`.
  - Save the session using `SessionManager.SaveSessionAsync`.
  - Query the `/rest/v1/profiles` table using the user's ID to fetch the `full_name`, `business_name`, and `avatar_url` (with `Authorization: Bearer <access_token>`).
  - Return `ApiResponse<AppUser>.Success` with the populated `AppUser`.
- **Register (`SignUpAsync`)**:
  - Send HTTP POST to `${Constants.AuthBase}/signup` with headers `apikey: Constants.SupabaseAnonKey` and `Content-Type: application/json`.
  - Extract the user `id` and any session token.
  - Send HTTP POST to `${Constants.RestBase}/profiles` with headers `apikey`, `Authorization: Bearer <token>`, and body containing `id`, `email`, and `full_name` to satisfy the database table constraint.
  - Save the session if access token is available.
  - Return `ApiResponse<AppUser>.Success`.
- **Sign Out (`SignOutAsync`)**:
  - Call `SessionManager.ClearSession()`.
- **Error Handling**:
  - Read response error JSON (handling GoTrue `{ error_description }`, `{ msg }`, or REST `{ message }` fields).
  - Throw clear user-friendly `Exception` containing the parsed error.

---

### 2. MVVM ViewModels Layer

#### [MODIFY] [LoginViewModel.cs](file:///c:/Users/Hp/Smart-Business-Manager/SmartBusinessManager/Features/Authentication/ViewModels/LoginViewModel.cs)
Implement manual MVVM properties and login commands:
- Inherit from `BaseViewModel`.
- Properties `Email` and `Password` with explicit backing fields and `OnPropertyChanged()` triggers.
- Helper property `HasError => !string.IsNullOrEmpty(ErrorMessage)`.
- Commands:
  - `LoginCommand`: Invokes `SignInAsync`. Sets `IsBusy` and wraps execution in a robust try-catch. If an exception is caught, sets `ErrorMessage` which the View displays.
  - `NavigateToRegisterCommand`: Navigates to `//register`.
- Set UI controls enabled status using `IsNotBusy`.

#### [MODIFY] [RegisterViewModel.cs](file:///c:/Users/Hp/Smart-Business-Manager/SmartBusinessManager/Features/Authentication/ViewModels/RegisterViewModel.cs)
Implement manual MVVM properties and registration commands:
- Inherit from `BaseViewModel`.
- Properties `Email` and `Password` with backing fields.
- Helper property `HasError => !string.IsNullOrEmpty(ErrorMessage)`.
- Commands:
  - `RegisterCommand`: Auto-generates `fullName` from `Email.Split('@')[0]`. Invokes `SignUpAsync`. Wraps execution in try-catch and populates `ErrorMessage` on failure. Navigates to `//dashboard` on success.
  - `NavigateToLoginCommand`: Navigates to `//login`.

---

### 3. Views Layer

#### [MODIFY] [LoginPage.xaml](file:///c:/Users/Hp/Smart-Business-Manager/SmartBusinessManager/Features/Authentication/Views/LoginPage.xaml)
Recreate the exact look of `authentication.html` for Sign In:
- Centered login card (`Border` with shadow, max width, white background, rounded corners 24).
- Brand Logo area (primary color rounded container with a business icon, title "Smart Manager", subtitle).
- Aesthetic tabs (A selected "Sign In" tab with a bottom primary indicator line, and an unselected "Create Account" tab that triggers `NavigateToRegisterCommand`).
- Input forms (Emails & Password fields with standard style tokens, mail and lock icons, and clean rounded border inputs).
- A Label bound to `ErrorMessage` (colored `Danger`, only visible when `HasError` is true).
- Primary CTA Button ("Sign In" text with trailing arrow icon and premium button shadow).
- An overlay `ActivityIndicator` bound to `IsBusy`.

#### [MODIFY] [LoginPage.xaml.cs](file:///c:/Users/Hp/Smart-Business-Manager/SmartBusinessManager/Features/Authentication/Views/LoginPage.xaml.cs)
- Set constructor injection for `LoginViewModel`.
- Implement basic password visibility toggle helper.

#### [MODIFY] [RegisterPage.xaml](file:///c:/Users/Hp/Smart-Business-Manager/SmartBusinessManager/Features/Authentication/Views/RegisterPage.xaml)
Recreate the exact look of `authentication.html` for registration:
- Centered register card (matching style of LoginPage).
- Brand Logo area.
- Tabs (An unselected "Sign In" tab that triggers `NavigateToLoginCommand`, and a selected "Create Account" tab with a bottom primary indicator line).
- Standard Email & Password inputs.
- Error label and loading indicator overlay.
- Primary CTA Button ("Create Account ➔").

#### [MODIFY] [RegisterPage.xaml.cs](file:///c:/Users/Hp/Smart-Business-Manager/SmartBusinessManager/Features/Authentication/Views/RegisterPage.xaml.cs)
- Set constructor injection for `RegisterViewModel`.
- Implement basic password visibility toggle helper.

---

## Verification Plan

### Automated Build Verification
- Run `.NET MAUI` build command to ensure zero compilation or syntax errors:
  `dotnet build`

### Manual & Visual Verification
- Deploy and review the UI in mobile simulation or standard view.
- Test login with valid and invalid credentials, verifying that the appropriate exception message is parsed and bound to the UI.
- Test signup with a new account, verifying that the profile is successfully created in Supabase.
