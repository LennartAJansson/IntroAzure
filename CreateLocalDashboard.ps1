#Tell kubectl to use local Docker Desktops Kubernetes container
kubectl config use-context docker-desktop 

#Add the dashboard from github
kubectl apply -f https://raw.githubusercontent.com/kubernetes/dashboard/v2.0.0-beta8/aio/deploy/recommended.yaml

#Setup security for the dashboard
kubectl apply -f .\LocalDashboard.yml

$secret = kubectl -n kubernetes-dashboard describe secret $(kubectl -n kubernetes-dashboard get secret | sls admin-user | ForEach-Object { $_ -Split '\s+' } | Select -First 1)
$secret | Out-File -FilePath ".\Logs\LocalKubernetesDashboardUser.txt"

".\Logs\LocalKubernetesDashboardUser.txt contains the token you need to logon to the dashboard. Remember to remove the spaces at the line endings!"
""
"As the next step, run the command 'kubectl proxy' in a separate cmd window"
"To release a hanged kubectl proxy kill the process that you get from netstat -ano for port 8001"
""
"Use your browser to goto the address:"
"http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/"
