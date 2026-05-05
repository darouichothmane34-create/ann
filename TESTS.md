# TESTS MANUELS — AnnuaireEntreprise (soutenance)

> Date de référence: 2026-05-04.

## Pré-requis
- Windows avec .NET 8 SDK installé
- Accès Internet (pour RandomUser)
- Projet: `AnnuaireEntreprise.Wpf`

## Démarrage
1. `cd AnnuaireEntreprise.Wpf`
2. `dotnet restore`
3. `dotnet build`
4. `dotnet run`

Résultat attendu:
- L'application se lance.
- La base SQLite est créée automatiquement si absente.

---

## Checklist (16 points)

- [ ] 1. **Lancement application**
  - Ouvrir l'application sans erreur.

- [ ] 2. **Recherche par nom partiel**
  - Saisir une partie du nom puis cliquer **Rechercher**.
  - Vérifier que la grille est filtrée.

- [ ] 3. **Recherche par site**
  - Choisir un site puis cliquer **Rechercher**.
  - Vérifier les salariés affichés.

- [ ] 4. **Recherche par service**
  - Choisir un service puis cliquer **Rechercher**.
  - Vérifier les salariés affichés.

- [ ] 5. **Affichage fiche salarié**
  - Sélectionner un salarié.
  - Vérifier le panneau détail (Nom, Prénom, tél fixe, tél portable, email, service, site).

- [ ] 6. **Mode caché Ctrl + Shift + A**
  - Appuyer sur `Ctrl+Shift+A`.
  - Vérifier ouverture de la fenêtre de login admin.

- [ ] 7. **Connexion admin correcte**
  - Mot de passe: `Admin123!`.
  - Vérifier ouverture de `AdminWindow`.

- [ ] 8. **Connexion admin refusée**
  - Saisir un mauvais mot de passe.
  - Vérifier le message d'erreur.

- [ ] 9. **CRUD Sites**
  - Ajouter, modifier, supprimer un site.
  - Vérifier rafraîchissement et messages.

- [ ] 10. **CRUD Services**
  - Ajouter, modifier, supprimer un service.
  - Vérifier rafraîchissement et messages.

- [ ] 11. **CRUD Salariés**
  - Ajouter, modifier, supprimer un salarié.
  - Vérifier les validations (nom/prénom/email/service/site).

- [ ] 12. **Génération PDF**
  - En mode visiteur, sélectionner un salarié puis cliquer **Générer PDF**.
  - Vérifier fichier dans `Documents/FichesSalaries`.

- [ ] 13. **Import API RandomUser**
  - En admin > onglet Salariés, cliquer **Importer API**.
  - Vérifier message indiquant le nombre importé.
  - Refaire une importation et vérifier limitation des doublons email.

- [ ] 14. **Logs admin**
  - Vérifier `Logs/admin-access.log`.
  - Attendus: tentative réussie, refusée, ouverture panneau admin.

- [ ] 15. **Logs erreurs**
  - Vérifier `Logs/errors.log`.
  - Attendus: erreurs CRUD/API/PDF/UI si déclenchées.

- [ ] 16. **Deux instances ouvertes en même temps**
  - Lancer 2 fois l'application.
  - Faire des opérations de lecture/écriture simples (ex: ajout site sur instance A, rafraîchir sur B).
  - Vérifier absence de conflit bloquant (WAL activé SQLite).

---

## Notes de validation soutenance
- Utiliser le bouton **Réinitialiser** entre scénarios de recherche.
- Conserver des captures d'écran des points critiques (admin, logs, PDF).
