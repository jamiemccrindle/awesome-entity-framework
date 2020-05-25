mkdir /home/vscode/.ssh
cp /opt/home/.ssh/id_rsa /opt/home/known_hosts /home/vscode/.ssh
cp /opt/home/.gitconfig /home/vscode/
sudo chown -R vscode /home/vscode
chmod 600 /home/vscode/.ssh/id_rsa