$services = kubectl get services -o=json | ConvertFrom-Json

foreach($item in $services.items | Where-Object {$_.metadata.name -eq "weatherforecast"})
{
"Service name: " + $item.metadata.name
"Namespace: " + $item.metadata.namespace
"Internal IP: " + $item.spec.clusterIp
"Public IP: " + $item.status.loadBalancer.ingress[0].ip
}