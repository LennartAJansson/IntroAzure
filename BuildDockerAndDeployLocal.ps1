#Make something that count the deploys so they will be numbered sequentially below

#Build the images needed
docker build -f .\OcelotGateway\Dockerfile . -t ocelotgateway:latest -t ocelotgateway:999
docker build -f .\WebApi\Dockerfile . -t webapi:latest -t webapi:999

#Tell kubectl to use local Docker Desktops Kubernetes container
kubectl config use-context docker-desktop 

#Apply the latest local images
kubectl apply -f .\LocalDeploy.yml

#List all deployments, pods and services
kubectl get deployments
kubectl get pods
kubectl get services

"kubectl delete -f .\LocalDeploy.yml will remove the deployment again"