https://learnk8s.io/blog/installing-docker-and-kubernetes-on-windows  
https://www.docker.com/blog/docker-windows-desktop-now-kubernetes/  


Download Docker for Windows:  
https://hub.docker.com/?overlay=onboarding  
Requires:
Windows 10 64-bit: Pro, Enterprise, or Education (Build 15063 or later).
Hyper-V and Containers Windows features must be enabled.

docker run -p 8000:80 -d nginx  
host.docker.internal  
gateway.docker.internal  



https://docs.docker.com/docker-for-windows/dashboard/  
https://docs.docker.com/docker-for-windows/kubernetes/  
https://kubernetes.io/docs/reference/kubectl/overview/  

kubectl config get-contexts  
kubectl config use-context docker-desktop  
kubectl get services  [-n namespace]  
kubectl get nodes  

https://docs.docker.com/docker-for-windows/troubleshoot/
