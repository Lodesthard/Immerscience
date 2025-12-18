# Immerscience

Ici est le git pour le projet Immerscience

# Manuel d'utilisation de git

1. Créer une nouvelle branche dans le github. Mettez votre nom ou la tâche que vous faites si vous n'aveez pas d'idée de nom pour la branche. (cliquer sur branch puis new branch). ATTENTION ⚠️ !!! NE SURTOUT PAS MODIFIER DIRECTEMENT LA BRANCHE PRINCIPALE.
2. Télécharger le git en local. Pour cela, créer un dossier dans votre PC et ouvrir le terminal de commande dans ce dossier. Mettre dans le terminal la commande : git clone https://github.com/Lodesthard/Immerscience.git puis : cd Immerscience puis : git checkout suivi du nom de votre branche pour aller dans votre branche.
3. Faire votre code en local.
4. Une fois votre session terminée afin d'upload votre code sur github revenez dans l'invite de commande dans votre dossier et faites git add . pour tout ajouter, git commit "nom du commit" puis git push origin suivi du nom de votre branche. ⚠️ !!! VERIFIEZ BIEN QUE LE TRAVAIL A ETE MIS SUR GITHUB;

# Vérifier le gitignore

aller dans le terminal de commandes et inscrire ces 2 commandes :

 - git check-ignore -v Library/
 - git check-ignore -v Temp/

Ceci devrait afficher respectivement :

Library/
Temp/
