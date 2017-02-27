# Deploying to Azure Container Service with Kubernetes

## Cluster setup

Create the Kubernetes cluster in ACS using the Azure CLI:

```bash
az acs create -n dockercoins-cluster -g dockercoins -d rmc-dockercoins --orchestrator-type kubernetes
```

Install the the kuberenetes CLI (kubectl):
```bash
az acs kubernetes install-cli
```

Get the kubectl config file fo rthe cluster:
```bash
az acs kubernetes get-credentials --dns-prefix=rmc-dockercoins --location=westeurope --user azureuser
```

## Deploy the application to the cluster

1. Create a Azure Redis Cache with the name: rmc-dockercoins.redis.cache.windows.net
2. Create namepace for the application:
```bash
kubectl create -f namespace.yaml
```
3. Set the default namespace for the following kubectl commands:
```bash
kubectl config set-context rmc-dockercoins --namespace=dockercoins-dev
```
4. Create the RNG & Hasher deployments and services:
```bash
kubectl create -f rng-deployment.yaml
kubectl create -f hasher-deployment.yaml
kubectl create -f rng-service.yaml
kubectl create -f hasher-service.yaml
```
5. Create the Redis config and secrets
```bash 
kubectl create secret generic cache-secrets --from-literal=password=**REDISPASSWORD**
kubectl create -f cache_config.yaml
```
6. Create the work deployment:
```bash
kubectl create -f worker-deployment.yaml
```
7. Create the webui deployment and service
```bash
kubectl create -f webui-deployment.yaml
kubectl create -f webui-service.yaml
```
This will create a load balancer in Azure (which will take some time to create)

## Monitoring Kuberenets
1. Start the local proxy:
```bash
kubectl proxy
```
2. Open browser to view (dashboard)[http://127.0.0.1:8001/api/v1/proxy/namespaces/kube-system/services/kubernetes-dashboard/#/workload?namespace=dockercoins-dev]

## Scaling the service
You can scale the service. For example:
```bash
kubectl scale --replicas=2 deployment/worker
```


