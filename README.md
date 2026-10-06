<div align="center">

#  TT - Training 

**Plateforme web de gestion des formations du personnel, avec assistant IA intégré**

![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-MVC-512BD4?logo=dotnet&logoColor=white)
![Entity Framework](https://img.shields.io/badge/EF_Core-Pomelo-512BD4?logo=dotnet&logoColor=white)
![MySQL](https://img.shields.io/badge/MySQL-MariaDB-4479A1?logo=mysql&logoColor=white)
![OpenAI](https://img.shields.io/badge/OpenAI-Assistant-412991?logo=openai&logoColor=white)
![Status](https://img.shields.io/badge/status-en_développement-orange)

</div>

---

## À propos

Application développée lors du stage de perfectionnement chez **Tunisie Télécom** (17 juin – 23 août 2026). Elle centralise la gestion du cycle de formation : planification des sessions, suivi des présences, justification des absences, reporting budgétaire et portail dédié aux employés.

Deux interfaces distinctes :

- **Espace Responsable / Agent TT** : administration complète des formations, formateurs, employés et rapports.
- **Portail Employé** : consultation de ses formations, historique, absences, notifications et profil.

## Fonctionnalités

| Module | Description |
|---|---|
| **Employés** | CRUD complet, photo de profil, authentification, changement de mot de passe |
| **Formateurs** | Gestion des formateurs internes et externes |
| **Formations** | Création, domaines, types, filtres, sessions et participants |
| **Présences** | Feuille d'appel par session, suivi des absences |
| **Justifications** | Dépôt de justificatifs par l'employé, validation par le responsable |
| **Reporting** | Rapports par formation, par employé et budgétaires |
| **Assistant IA** | Questions en langage naturel sur les données de formation (OpenAI) |
| **Notifications** | Alertes envoyées aux employés (inscription, absence, etc.) |

## Stack technique

- **Backend** : ASP.NET Core MVC (C#), Entity Framework Core (Pomelo)
- **Base de données** : MySQL / MariaDB (XAMPP)
- **Frontend** : Razor Views, CSS personnalisé (thème mauve / marine), JavaScript
- **IA** : API OpenAI
- **Sécurité** : User Secrets pour les clés et chaînes de connexion

## Structure du projet

```
Training_tunisie_telecome/
├── Controllers/      # Logique MVC (Formations, Attendance, Reporting, EmployeePortal...)
├── Models/           # Entités et ViewModels
├── Data/             # AppDbContext
├── Migrations/       # Migrations EF Core
├── Services/         # OpenAIService
├── Views/            # Vues Razor
└── wwwroot/          # Fichiers statiques et uploads
```

## Installation

### Prérequis

- [.NET SDK](https://dotnet.microsoft.com/download) (version du projet)
- [XAMPP](https://www.apachefriends.org/) (MySQL / MariaDB démarré)
- Visual Studio 2022 ou plus récent (recommandé)

### Étapes

```bash
# 1. Cloner le dépôt
git clone https://github.com/nourelhoudaachour2/TT-Training-ASPNET.git
cd TT-Training-ASPNET

# 2. Configurer les secrets (jamais commités)
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "server=localhost;database=telecome;user=root;password=;"
dotnet user-secrets set "OpenAI:ApiKey" "VOTRE_CLE_API"

# 3. Créer la base de données
dotnet ef database update

# 4. Lancer l'application
dotnet run
```

L'application est ensuite accessible sur `https://localhost:xxxx` (port indiqué dans la console).

> La clé OpenAI est optionnelle : sans elle, seul l'assistant IA est désactivé.

## Captures d'écran

<!-- Ajoutez vos images dans docs/screenshots puis décommentez -->
<!--
| Dashboard | Formations |
|---|---|
| ![Dashboard](docs/screenshots/dashboard.png) | ![Formations](docs/screenshots/formations.png) |
-->

## Feuille de route

- [x] Gestion des employés, formateurs et formations
- [x] Sessions, présences et justifications
- [x] Reporting et budget
- [x] Portail employé
- [x] Assistant IA
- [ ] Export PDF / Excel des rapports
- [ ] Tests unitaires
- [ ] Déploiement

## Auteure

**Nour El Houda Achour**
Étudiante ingénieure en Génie Logiciel et Applications, IT Business School (ITBS) Nabeul

[![GitHub](https://img.shields.io/badge/GitHub-nourelhoudaachour2-181717?logo=github)](https://github.com/nourelhoudaachour2)

---

<div align="center">
Projet académique réalisé dans le cadre d'un stage chez Tunisie Télécom
</div>
