# Smart Business Manager — System Blueprint v1.2

> Single Source of Truth · Technical Reference · AI-Ready

---

## Project Metadata

| Field   | Value                            |
| ------- | -------------------------------- |
| Version | 1.2.0                            |
| Status  | Active — Locked for Development  |
| Stack   | MAUI · Supabase · SQL Instructions |
| Pattern | Manual MVVM · Shell Navigation   |
| Date    | 2026                             |

> هذا الملف هو المرجع التقني الوحيد للمشروع.

---

# 1. Project Overview

## 1.1 Vision

A cross-platform mobile application (iOS & Android) that enables small business owners, freelancers, and sales representatives to manage client relationships, log interactions, track payments, and receive AI-powered follow-up insights — all from a single interface.

---

## 1.2 Core Problem

1. Missed follow-up opportunities due to no centralized tracking.
2. Weakened customer relationships from inconsistent communication.
3. Financial leakage caused by untracked pending payments.

---

## 1.3 Architectural Decisions Log

> **These decisions are LOCKED. Do not deviate without a team decision and Blueprint update.**

| #    | Decision            | Choice Made                    | Reason                            |
| ---- | ------------------- | ------------------------------ | --------------------------------- |
| D-01 | UI Framework        | MAUI (Cross-platform)          | Single codebase for iOS & Android |
| D-02 | Backend             | Supabase (DB + Auth + Storage) | BaaS — minimal server ops         |
| D-03 | MVVM Implementation | Manual INotifyPropertyChanged  | No extra dependency, full control |
| D-04 | Navigation          | AppShell (Shell Navigation)    | Built-in MAUI, route-based        |
| D-05 | Supabase Client     | HttpClient (REST calls)        | No SDK dependency, transparent    |
| D-06 | AI Engine           | SQL Instructions (Database)      | Deterministic, executes inside DB |
| D-07 | Offline Support     | None (Online only)             | MVP scope reduction               |
| D-08 | Currency            | USD only (fixed)               | MVP simplification                |
| D-09 | Design Reference    | Figma (external)               | Blueprint defines tokens only     |
| D-10 | Shared UI Library   | Stateless Custom Controls      | MVVM-friendly, zero-dependency    |

---

# 2. Tech Stack & Dependencies

## 2.1 NuGet Packages (MAUI Project)

> **Only these packages are approved. Adding new packages requires team lead approval.**

| Package                                  | Version       | Purpose                       | Used By        |
| ---------------------------------------- | ------------- | ----------------------------- | -------------- |
| CommunityToolkit.Maui                    | Latest stable | Converters, Behaviors, Popups | All Features   |
| Supabase.Gotrue                          | Latest stable | Auth Token Management         | Core/Services  |
| Microsoft.Extensions.DependencyInjection | Built-in MAUI | DI Container                  | MauiProgram.cs |

---

## 2.2 Supabase Services Used

| Service               | How Used                                                     | Access Method             |
| --------------------- | ------------------------------------------------------------ | ------------------------- |
| PostgreSQL (DB)       | All data storage — clients, payments, interactions, insights | REST API via HttpClient   |
| Auth (GoTrue)         | User registration & login (JWT tokens)                       | REST API via HttpClient   |
| SQL Instructions (Database) | AI insight generation — Rule-Based logic                     | Database internal execution |
| Row Level Security    | Every table has RLS — users see only their data              | Auto-enforced by Supabase |

---

# 3. Project Structure

## 3.1 Folder Tree

> **Every developer MUST follow this exact structure. No new top-level folders without approval.**

```text
SmartBusinessManager/
│
├── Features/
│   ├── Authentication/
│   │   ├── Models/          
│   │   ├── ViewModels/      
│   │   └── Views/           
│   │
│   ├── Clients/
│   │   ├── Models/          
│   │   ├── ViewModels/      
│   │   └── Views/           
│   │
│   ├── Dashboard/
│   │   ├── ViewModels/      
│   │   └── Views/           
│   │
│   ├── Finance/
│   │   ├── Models/          
│   │   ├── ViewModels/      
│   │   └── Views/           
│   │
│   └── AI/
│       ├── ViewModels/      
│       └── Views/           
│
├── Core/
│   ├── Services/
│   │   ├── ISupabaseService.cs
│   │   ├── SupabaseService.cs
│   │   ├── IAuthService.cs
│   │   ├── AuthService.cs
│   │   ├── IAiService.cs
│   │   └── AiService.cs
│   │
│   ├── Models/
│   │   ├── ApiResponse.cs
│   │   └── AppUser.cs
│   │
│   └── Helpers/
│       ├── Constants.cs
│       └── SessionManager.cs
│
├── Resources/
│   └── Styles/
│       ├── Colors.xaml
│       └── Styles.xaml
│
├── Shared/
│   └── Components/
│       ├── Cards/          
│       ├── Common/         
│       ├── Forms/          
│       ├── Navigation/     
│       └── Views/          
│
├── AppShell.xaml
└── MauiProgram.cs
```

---

## 3.2 File Naming Convention

| Type                   | Convention                       | Example                  |
| ---------------------- | -------------------------------- | ------------------------ |
| Page (View)            | PascalCase + Page.xaml           | ClientDetailPage.xaml    |
| ViewModel              | Same name as Page + ViewModel.cs | ClientDetailViewModel.cs |
| Model                  | PascalCase (noun)                | Client.cs, Payment.cs    |
| Service Interface      | I + PascalCase + Service.cs      | ISupabaseService.cs      |
| Service Implementation | PascalCase + Service.cs          | SupabaseService.cs       |
| Constants              | PascalCase + Constants.cs        | Constants.cs             |

---

# 4. Database Schema (Supabase)

## 4.1 Enum Types

```sql
-- client_status
CREATE TYPE client_status AS ENUM (
  'active', 'inactive', 'lead', 'archived'
);

-- payment_status
CREATE TYPE payment_status AS ENUM (
  'pending', 'paid', 'overdue', 'cancelled', 'partial'
);

-- insight_type
CREATE TYPE insight_type AS ENUM (
  'follow_up_needed',
  'overdue_payment',
  'high_value_client',
  'inactive_client',
  'payment_due_soon'
);
```

---

## 4.2 Tables

### profiles

| Column        | Type        | Constraints             | Notes                 |
| ------------- | ----------- | ----------------------- | --------------------- |
| id            | uuid        | PK, FK → auth.users(id) | Supabase Auth user ID |
| full_name     | text        | NOT NULL                |                       |
| email         | text        | NOT NULL, UNIQUE        |                       |
| business_name | text        | nullable                |                       |
| phone         | text        | nullable                |                       |
| avatar_url    | text        | nullable                |                       |
| created_at    | timestamptz | NOT NULL DEFAULT now()  |                       |
| updated_at    | timestamptz | NOT NULL DEFAULT now()  |                       |

---

### clients

| Column            | Type          | Constraints                   | Notes                       |
| ----------------- | ------------- | ----------------------------- | --------------------------- |
| id                | uuid          | PK DEFAULT uuid_generate_v4() |                             |
| owner_id          | uuid          | NOT NULL, FK → profiles(id)   | RLS enforced on this        |
| name              | text          | NOT NULL                      |                             |
| phone             | text          | nullable                      |                             |
| email             | text          | nullable                      |                             |
| company           | text          | nullable                      |                             |
| notes             | text          | nullable                      |                             |
| status            | client_status | NOT NULL DEFAULT 'active'     | Enum                        |
| last_contacted_at | timestamptz   | nullable                      | Updated on each interaction |
| created_at        | timestamptz   | NOT NULL DEFAULT now()        |                             |
| updated_at        | timestamptz   | NOT NULL DEFAULT now()        |                             |

---

### interactions

| Column        | Type             | Constraints                   | Notes                           |
| ------------- | ---------------- | ----------------------------- | ------------------------------- |
| id            | uuid             | PK DEFAULT uuid_generate_v4() |                                 |
| client_id     | uuid             | NOT NULL, FK → clients(id)    |                                 |
| owner_id      | uuid             | NOT NULL, FK → profiles(id)   |                                 |
| type          | interaction_type | NOT NULL                      | call / meeting / message / note |
| subject       | text             | nullable                      |                                 |
| notes         | text             | nullable                      |                                 |
| interacted_at | timestamptz      | NOT NULL DEFAULT now()        |                                 |
| created_at    | timestamptz      | NOT NULL DEFAULT now()        |                                 |

---

### payments

| Column           | Type           | Constraints                   | Notes                    |
| ---------------- | -------------- | ----------------------------- | ------------------------ |
| id               | uuid           | PK DEFAULT uuid_generate_v4() |                          |
| client_id        | uuid           | NOT NULL, FK → clients(id)    |                          |
| owner_id         | uuid           | NOT NULL, FK → profiles(id)   |                          |
| amount           | numeric        | NOT NULL, CHECK (amount > 0)  |                          |
| currency         | varchar(3)     | NOT NULL DEFAULT 'USD'        | Fixed to USD in MVP      |
| status           | payment_status | NOT NULL DEFAULT 'pending'    | Enum                     |
| description      | text           | nullable                      |                          |
| due_date         | date           | nullable                      |                          |
| paid_at          | timestamptz    | nullable                      | Set when status → 'paid' |
| reference_number | text           | nullable                      |                          |
| created_at       | timestamptz    | NOT NULL DEFAULT now()        |                          |
| updated_at       | timestamptz    | NOT NULL DEFAULT now()        |                          |

---

### ai_insights

| Column       | Type         | Constraints                   | Notes                       |
| ------------ | ------------ | ----------------------------- | --------------------------- |
| id           | uuid         | PK DEFAULT uuid_generate_v4() |                             |
| client_id    | uuid         | NOT NULL, FK → clients(id)    |                             |
| owner_id     | uuid         | NOT NULL, FK → profiles(id)   |                             |
| insight_type | insight_type | NOT NULL                      | Enum — see Section 4.1      |
| message      | text         | NOT NULL                      | Human-readable insight text |
| priority     | smallint     | NOT NULL DEFAULT 3, 1–5       | 1 = highest priority        |
| is_read      | boolean      | NOT NULL DEFAULT false        |                             |
| generated_at | timestamptz  | NOT NULL DEFAULT now()        |                             |
| expires_at   | timestamptz  | nullable                      | SQL Instruction sets this     |

> **Schema Bug Fixed:** currency column type was `character` (1 char). Corrected to `varchar(3)` to support ISO codes.

---

# 5. Architecture & Patterns

## 5.1 Manual MVVM — Rules

> **We use Manual MVVM (no Community Toolkit). Every ViewModel inherits BaseViewModel below.**

```csharp
// Core/Models/BaseViewModel.cs
public class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set { _isBusy = value; OnPropertyChanged(); }
    }

    private string _title;
    public string Title
    {
        get => _title;
        set { _title = value; OnPropertyChanged(); }
    }

    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
```

---

## 5.2 Command Pattern

```csharp
// Use Command in ViewModel — NOT code-behind
public Command LoadClientsCommand { get; }

public ClientListViewModel(ISupabaseService supabaseService)
{
    _supabaseService = supabaseService;
    LoadClientsCommand = new Command(async () => await LoadClientsAsync());
}

private async Task LoadClientsAsync()
{
    if (IsBusy) return;

    IsBusy = true;

    try
    {
        /* ... */
    }
    catch (Exception ex)
    {
        /* handle */
    }
    finally
    {
        IsBusy = false;
    }
}
```

---

## 5.3 Shell Navigation — Route Registry

> **ALL routes MUST be registered in AppShell.xaml. Never use code-behind navigation.**

```xml
<!-- AppShell.xaml -->
<Shell>
    <TabBar>
        <Tab Title="Dashboard">
            <ShellContent Route="dashboard"
                          ContentTemplate="{DataTemplate views:DashboardPage}"/>
        </Tab>

        <Tab Title="Clients">
            <ShellContent Route="clients"
                          ContentTemplate="{DataTemplate views:ClientListPage}"/>
        </Tab>

        <Tab Title="Finance">
            <ShellContent Route="payments"
                          ContentTemplate="{DataTemplate views:PaymentListPage}"/>
        </Tab>

        <Tab Title="AI Insights">
            <ShellContent Route="insights"
                          ContentTemplate="{DataTemplate views:InsightsPage}"/>
        </Tab>
    </TabBar>
</Shell>

<!-- Register detail routes in MauiProgram.cs -->
Routing.RegisterRoute("clients/detail", typeof(ClientDetailPage));
Routing.RegisterRoute("clients/add", typeof(AddClientPage));
Routing.RegisterRoute("payments/add", typeof(AddPaymentPage));
```

---

### Navigation Cheat Sheet

| Action       | Code                                                                                                  |
| ------------ | ----------------------------------------------------------------------------------------------------- |
| Go to detail | `await Shell.Current.GoToAsync("clients/detail", new Dictionary<string,object>{{ "ClientId", id }});` |
| Go back      | `await Shell.Current.GoToAsync("..");`                                                                |
| Pass object  | `[QueryProperty("Client", "Client")] in ViewModel`                                                    |

---

# 6. Services Layer (HttpClient → Supabase)

## 6.1 Constants

```csharp
// Core/Helpers/Constants.cs
public static class Constants
{
    public const string SupabaseUrl =
        "https://YOUR_PROJECT.supabase.co";

    public const string SupabaseAnonKey =
        "YOUR_ANON_KEY";
}
```

---

## 6.2 SupabaseService Template

```csharp
// Core/Services/SupabaseService.cs
public class SupabaseService : ISupabaseService
{
    private readonly HttpClient _http;
    private readonly SessionManager _session;

    public SupabaseService(SessionManager session)
    {
        _session = session;
        _http = new HttpClient();

        _http.DefaultRequestHeaders.Add(
            "apikey",
            Constants.SupabaseAnonKey
        );
    }

    private void SetAuthHeader()
    {
        var token = _session.GetToken();

        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    // GET all clients for current user
    public async Task<List<Client>> GetClientsAsync()
    {
        SetAuthHeader();

        var url =
            $"{Constants.SupabaseUrl}/rest/v1/clients?select=*&order=created_at.desc";

        var resp = await _http.GetAsync(url);

        resp.EnsureSuccessStatusCode();

        var json = await resp.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<List<Client>>(json);
    }

    // POST — insert new client
    public async Task AddClientAsync(Client client)
    {
        SetAuthHeader();

        var url =
            $"{Constants.SupabaseUrl}/rest/v1/clients";

        var content = new StringContent(
            JsonSerializer.Serialize(client),
            Encoding.UTF8,
            "application/json"
        );

        content.Headers.Add("Prefer", "return=minimal");

        var resp = await _http.PostAsync(url, content);

        resp.EnsureSuccessStatusCode();
    }
}
```

---

## 6.3 Supabase REST API Reference

| Operation      | HTTP Method | URL Pattern                          | Headers                                       |
| -------------- | ----------- | ------------------------------------ | --------------------------------------------- |
| Get all rows   | GET         | `/rest/v1/{table}?select=*`          | apikey, Authorization                         |
| Filter rows    | GET         | `/rest/v1/{table}?column=eq.value`   | apikey, Authorization                         |
| Insert row     | POST        | `/rest/v1/{table}`                   | apikey, Authorization, Prefer: return=minimal |
| Update row     | PATCH       | `/rest/v1/{table}?id=eq.{id}`        | apikey, Authorization                         |
| Delete row     | DELETE      | `/rest/v1/{table}?id=eq.{id}`        | apikey, Authorization                         |
| Auth – Sign Up | POST        | `/auth/v1/signup`                    | apikey                                        |
| Auth – Sign In | POST        | `/auth/v1/token?grant_type=password` | apikey                                        |

---

# 7. AI Module — Rule-Based Insights

## 7.1 How It Works

The AI module is powered by SQL Instructions executed directly within the Supabase database (e.g., using Postgres functions or triggers). It does NOT call any LLM API. It evaluates data within the database and maintains structured insights in the `ai_insights` table. The MAUI application simply reads from this table to display insights.

---

## 7.2 Business Rules (The 5 Insight Types)

| insight_type      | Trigger Condition                                                         | Priority | Example Message                                                 |
| ----------------- | ------------------------------------------------------------------------- | -------- | --------------------------------------------------------------- |
| follow_up_needed  | `last_contacted_at > 14 days ago AND status = 'active'`                   | 2        | Ahmed hasn't been contacted for 14 days. Consider following up. |
| overdue_payment   | `payment.status = 'overdue' OR (due_date < today AND status = 'pending')` | 1        | Sara has an overdue payment of $500. Follow up now.             |
| payment_due_soon  | `due_date is within next 3 days AND status = 'pending'`                   | 2        | Payment from Khaled is due in 2 days.                           |
| inactive_client   | `last_contacted_at > 30 days AND no pending payments`                     | 3        | No activity with Ahmed for 30+ days.                            |
| high_value_client | `total paid in last 90 days > $1,000`                                     | 4        | Ali is a high-value client. Prioritize relationship.            |

---

## 7.3 SQL Instruction Structure

```sql
-- Example Postgres function to generate insights (Runs periodically or via trigger)
CREATE OR REPLACE FUNCTION generate_ai_insights()
RETURNS void AS $$
BEGIN
  -- Clear expired or old insights
  DELETE FROM ai_insights WHERE expires_at < now();

  -- Rule: follow_up_needed
  INSERT INTO ai_insights (client_id, owner_id, insight_type, message, priority, expires_at)
  SELECT 
    id, owner_id, 'follow_up_needed', 
    name || ' hasn''t been contacted for over 14 days. Consider following up.', 
    2, now() + interval '7 days'
  FROM clients
  WHERE status = 'active' AND last_contacted_at < now() - interval '14 days'
  ON CONFLICT DO NOTHING;

  -- ... (repeat for each rule)
END;
$$ LANGUAGE plpgsql;
```

---

## 7.4 Trigger Point (MAUI App Side)

Since the insights are generated inside the database, the MAUI app only needs to fetch the available results when the user opens the `Insights` page (`InsightsViewModel.cs`):

```csharp
public async Task RefreshInsightsAsync()
{
    IsBusy = true;

    // Fetch the pre-generated results directly from the database
    Insights = await _supabaseService.GetInsightsAsync();

    IsBusy = false;
}
```

---

# 8. Dependency Injection — MauiProgram.cs

> **Every Service and ViewModel MUST be registered here. No manual `new` outside of this file.**

```csharp
// MauiProgram.cs

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(...);

        // ── Core Services (Singleton)
        builder.Services.AddSingleton<SessionManager>();

        builder.Services.AddSingleton<
            ISupabaseService,
            SupabaseService>();

        builder.Services.AddSingleton<
            IAuthService,
            AuthService>();

        builder.Services.AddSingleton<
            IAiService,
            AiService>();

        // ── ViewModels (Transient)
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<ClientListViewModel>();
        builder.Services.AddTransient<ClientDetailViewModel>();
        builder.Services.AddTransient<PaymentListViewModel>();
        builder.Services.AddTransient<AddPaymentViewModel>();
        builder.Services.AddTransient<InsightsViewModel>();

        // ── Pages (Transient)
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<ClientListPage>();
        builder.Services.AddTransient<ClientDetailPage>();
        builder.Services.AddTransient<PaymentListPage>();
        builder.Services.AddTransient<InsightsPage>();

        return builder.Build();
    }
}
```

---

# 9. C# Data Models

## 9.1 Client.cs

```csharp
// Features/Clients/Models/Client.cs

using System.Text.Json.Serialization;

public class Client
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("owner_id")]
    public string OwnerId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("phone")]
    public string Phone { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("company")]
    public string Company { get; set; }

    [JsonPropertyName("notes")]
    public string Notes { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("last_contacted_at")]
    public DateTime? LastContactedAt { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}
```

---

## 9.2 Payment.cs

```csharp
// Features/Finance/Models/Payment.cs

public class Payment
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("client_id")]
    public string ClientId { get; set; }

    [JsonPropertyName("owner_id")]
    public string OwnerId { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "USD";

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("due_date")]
    public DateOnly? DueDate { get; set; }

    [JsonPropertyName("paid_at")]
    public DateTime? PaidAt { get; set; }

    [JsonPropertyName("reference_number")]
    public string ReferenceNumber { get; set; }
}
```

---

## 9.3 AiInsight.cs

```csharp
// Features/AI/Models/AiInsight.cs

public class AiInsight
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("client_id")]
    public string ClientId { get; set; }

    [JsonPropertyName("insight_type")]
    public string InsightType { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    [JsonPropertyName("is_read")]
    public bool IsRead { get; set; }

    [JsonPropertyName("generated_at")]
    public DateTime GeneratedAt { get; set; }
}
```

---

# 10. Design Tokens (Colors.xaml)

> **These tokens mirror the Figma design system. Any UI color MUST come from this file. No hardcoded HEX values in XAML.**

<!-- Resources/Styles/Colors.xaml -->

<ResourceDictionary>

    <!-- ========================================= -->
    <!-- Primary Brand -->
    <!-- ========================================= -->

    <Color x:Key="Primary">#3525CD</Color>
    <Color x:Key="OnPrimary">#FFFFFF</Color>

    <Color x:Key="PrimaryContainer">#4F46E5</Color>
    <Color x:Key="OnPrimaryContainer">#DAD7FF</Color>

    <Color x:Key="PrimaryFixed">#E2DFFF</Color>
    <Color x:Key="PrimaryFixedDim">#C3C0FF</Color>

    <Color x:Key="InversePrimary">#C3C0FF</Color>

    <!-- ========================================= -->
    <!-- Secondary -->
    <!-- ========================================= -->

    <Color x:Key="Secondary">#006C49</Color>
    <Color x:Key="OnSecondary">#FFFFFF</Color>

    <Color x:Key="SecondaryContainer">#6CF8BB</Color>
    <Color x:Key="OnSecondaryContainer">#00714D</Color>

    <!-- ========================================= -->
    <!-- Tertiary / Warning Accent -->
    <!-- ========================================= -->

    <Color x:Key="Tertiary">#7E3000</Color>
    <Color x:Key="OnTertiary">#FFFFFF</Color>

    <Color x:Key="TertiaryContainer">#A44100</Color>
    <Color x:Key="OnTertiaryContainer">#FFD2BE</Color>

    <!-- ========================================= -->
    <!-- Error -->
    <!-- ========================================= -->

    <Color x:Key="Error">#BA1A1A</Color>
    <Color x:Key="OnError">#FFFFFF</Color>

    <Color x:Key="ErrorContainer">#FFDAD6</Color>
    <Color x:Key="OnErrorContainer">#93000A</Color>

    <!-- ========================================= -->
    <!-- Surface System -->
    <!-- ========================================= -->

    <Color x:Key="Background">#FCF8FF</Color>
    <Color x:Key="OnBackground">#1B1B24</Color>

    <Color x:Key="Surface">#FCF8FF</Color>
    <Color x:Key="SurfaceDim">#DCD8E5</Color>
    <Color x:Key="SurfaceBright">#FCF8FF</Color>

    <Color x:Key="SurfaceContainerLowest">#FFFFFF</Color>
    <Color x:Key="SurfaceContainerLow">#F5F2FF</Color>
    <Color x:Key="SurfaceContainer">#F0ECF9</Color>
    <Color x:Key="SurfaceContainerHigh">#EAE6F4</Color>
    <Color x:Key="SurfaceContainerHighest">#E4E1EE</Color>

    <Color x:Key="SurfaceVariant">#E4E1EE</Color>

    <!-- ========================================= -->
    <!-- Surface Text -->
    <!-- ========================================= -->

    <Color x:Key="OnSurface">#1B1B24</Color>
    <Color x:Key="OnSurfaceVariant">#464555</Color>

    <Color x:Key="InverseSurface">#302F39</Color>
    <Color x:Key="InverseOnSurface">#F3EFFC</Color>

    <!-- ========================================= -->
    <!-- Borders & Outlines -->
    <!-- ========================================= -->

    <Color x:Key="Outline">#777587</Color>
    <Color x:Key="OutlineVariant">#C7C4D8</Color>

    <!-- ========================================= -->
    <!-- Semantic Aliases -->
    <!-- ========================================= -->

    <!-- Success -->
    <Color x:Key="Success">#006C49</Color>
    <Color x:Key="SuccessBg">#6CF8BB</Color>

    <!-- Warning -->
    <Color x:Key="Warning">#A44100</Color>
    <Color x:Key="WarningBg">#FFD2BE</Color>

    <!-- Danger -->
    <Color x:Key="Danger">#BA1A1A</Color>
    <Color x:Key="DangerBg">#FFDAD6</Color>

    <!-- ========================================= -->
    <!-- Legacy Compatibility -->
    <!-- ========================================= -->

    <!-- Keep these temporarily if old UI depends on them -->

    <Color x:Key="TextPrimary">#1B1B24</Color>
    <Color x:Key="TextSecondary">#464555</Color>
    <Color x:Key="TextMuted">#777587</Color>

    <Color x:Key="Border">#C7C4D8</Color>
    <Color x:Key="White">#FFFFFF</Color>

</ResourceDictionary>

---

# 11. Error Handling Strategy

## 11.1 Rules

1. ALL async calls must be wrapped in try/catch.
2. Services throw exceptions — ViewModels catch them.
3. User-facing errors: display via a simple `ErrorMessage` property bound to a `Label` in the View.
4. Never show raw exception messages to the user.

---

## 11.2 Pattern

```csharp
// In ViewModel

private string _errorMessage;

public string ErrorMessage
{
    get => _errorMessage;
    set
    {
        _errorMessage = value;
        OnPropertyChanged();
    }
}

private async Task LoadAsync()
{
    IsBusy = true;
    ErrorMessage = null;

    try
    {
        Clients = await _supabaseService.GetClientsAsync();
    }
    catch (HttpRequestException)
    {
        ErrorMessage =
            "Network error. Please check your connection.";
    }
    catch (Exception)
    {
        ErrorMessage =
            "Something went wrong. Please try again.";
    }
    finally
    {
        IsBusy = false;
    }
}
```

---

# 12. Team Rules & Git Workflow

## 12.1 Branch Strategy

| Branch                  | Purpose                                      | Who Creates    |
| ----------------------- | -------------------------------------------- | -------------- |
| main                    | Production-ready only. Never push directly.  | CI/CD only     |
| develop                 | Integration branch. All features merge here. | Always exists  |
| feature/{module}-{task} | e.g. feature/clients-add-page                | Each developer |
| fix/{description}       | Bug fixes only                               | Each developer |

---

## 12.2 Commit Convention

```text
feat(clients): add client list page and viewmodel
fix(auth): handle token expiry on 401 response
style(dashboard): adjust card spacing
refactor(services): extract HttpClient setup to base class
docs(blueprint): update DI registration section
```

---

## 12.3 Module Ownership

| Module           | ClickUp List         | Scope                                         |
| ---------------- | -------------------- | --------------------------------------------- |
| Authentication   | Authentication       | Login, Register, Token Storage                |
| Client Module    | Client Module        | CRUD + Interactions + Client History          |
| Financial Module | Financial Module     | Payments CRUD + Status Management             |
| Dashboard Module | Dashboard Module     | KPIs + Recent Activity Feed                   |
| AI Module        | AI Module            | Insights Page + SQL Instructions              |
| Core / Shared    | Core / Shared Module | BaseViewModel, Services, Constants, Helpers, UI Components |
| Backend          | Backend              | Supabase Schema, RLS Policies, SQL Instructions |

> **If your task touches another module's files — check with that module's owner FIRST.**

---

# 13. Changelog

| Version | Date | Change                                    | Author    |
| ------- | ---- | ----------------------------------------- | --------- |
| 1.2.0   | 2026 | Implemented core Shared UI Components (Stateless, MVVM-ready library). | Team Lead |
| 1.1.0   | 2026 | Switched AI Module from Edge Functions to SQL Instructions. | Team Lead |
| 1.0.0   | 2026 | Initial Blueprint — all decisions locked. | Team Lead |
