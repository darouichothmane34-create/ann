# Annuaire d’entreprise — INFDI2 (C# WPF)

## 1) Présentation du projet
Ce projet est une **application lourde (desktop)** d’annuaire d’entreprise, réalisée en **C# .NET 8 / WPF**.

### Contexte métier
L’entreprise cible est une structure de l’**industrie agroalimentaire** disposant de plusieurs sites en France (ex: Paris, Lyon, Marseille…).

### Objectif
- Permettre aux utilisateurs de **rechercher des collaborateurs** rapidement.
- Permettre aux administrateurs de **gérer les données** (sites, services, salariés).

---

## 2) Fonctionnalités visiteur
Le mode visiteur permet :
- Recherche par **nom partiel**
- Recherche par **site**
- Recherche par **service**
- Affichage de la **fiche détaillée salarié**
- **Génération PDF** d’une fiche salarié

---

## 3) Fonctionnalités administrateur
Le mode administrateur inclut :
- Accès caché via **Ctrl + Shift + A**
- Authentification par mot de passe
- CRUD **Sites**
- CRUD **Services**
- CRUD **Salariés**
- Logs des accès administrateur

> Mot de passe admin par défaut : `Admin123!`

---

## 4) Base de données
- SGBD : **SQLite**
- Tables principales :
  - `Sites`
  - `ServicesEntreprise`
  - `Salaries`
- Relations :
  - Un site possède plusieurs salariés
  - Un service possède plusieurs salariés
- Concurrence : activation du mode **WAL** (Write-Ahead Logging) pour faciliter l’usage de plusieurs instances.

---

## 5) Choix techniques argumentés
- **C# .NET 8** : version moderne, stable, performante
- **WPF** : adapté aux applications desktop riches
- **SQLite** : léger, simple à déployer, idéal pour projet pédagogique
- **Entity Framework Core** : ORM productif pour le modèle objet/relationnel
- **Requêtes SQL primitives** : démonstration de maîtrise SQL bas niveau (Sites/Services)
- **QuestPDF** : génération de documents PDF structurés
- **API RandomUser** : intégration d’un service distant public pour enrichir les données
- **Architecture orientée objet** : séparation claire des responsabilités

---

## 6) Architecture du projet
- `AnnuaireEntreprise.Wpf/Models/` : entités métier (`Site`, `ServiceEntreprise`, `Salarie`)
- `AnnuaireEntreprise.Wpf/Data/` : contexte EF Core et initialisation base
- `AnnuaireEntreprise.Wpf/Repositories/` : accès aux données (EF Core + SQL primitives)
- `AnnuaireEntreprise.Wpf/Services/` : services transverses (auth, logs, PDF, import API)
- `AnnuaireEntreprise.Wpf/ViewModels/` : logique MVVM des écrans
- Vues XAML : fichiers `AnnuaireEntreprise.Wpf/*.xaml` (fenêtres/écrans)
- `AnnuaireEntreprise.Wpf/Helpers/` : utilitaires (ex: `RelayCommand`)
- `Logs/` : journaux d’exécution (`admin-access.log`, `errors.log`)
- `Documents/` : génération des PDF (`FichesSalaries`)

---

## 7) Méthode de gestion de projet
Le projet a été mené en mode **Kanban** avec :
- Découpage en étapes fonctionnelles successives
- Commits réguliers et lisibles
- Suivi continu dans le repository GitHub

---

## 8) Installation
### Prérequis
- Windows 10/11
- .NET SDK 8

### Étapes
```bash
git clone <URL_DU_REPO>
cd AnnuaireEntreprise.Wpf
dotnet restore
dotnet build
dotnet run
```

---

## 9) Utilisation
### Mode visiteur
- Rechercher des salariés (nom/site/service)
- Consulter les détails
- Générer une fiche PDF

### Mode administrateur
- Ouvrir le login via `Ctrl + Shift + A`
- Saisir le mot de passe admin (`Admin123!`)
- Gérer Sites, Services, Salariés
- Importer des salariés depuis l’API RandomUser

---

## 10) Tests
Les tests manuels détaillés sont documentés dans **`TESTS.md`**.

---

## 11) Scénario de démonstration (15 minutes)
1. **Présentation rapide** (objectif et contexte)
2. **Interface visiteur** (recherche + détail)
3. **Mode admin caché** (`Ctrl + Shift + A`)
4. **CRUD** Sites/Services/Salariés
5. **Génération PDF** d’une fiche salarié
6. **Import API** RandomUser
7. **Git/Kanban** (historique des étapes et commits)
8. **Conclusion**

---

## 12) Conclusion
Ce projet répond au cahier des charges INFDI2 :
- Application lourde avec interface graphique
- Base relationnelle SQLite
- CRUD complet
- ORM + SQL primitives
- Rôles visiteur / administrateur
- Logs
- Génération PDF
- Intégration API distante

Il est prêt pour une soutenance structurée, avec une démonstration claire des exigences techniques et fonctionnelles.
