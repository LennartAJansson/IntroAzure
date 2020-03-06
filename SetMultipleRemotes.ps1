#DO NOT USE THIS SCRIPT, IT IS INTENDED TO CREATE MULTIPLE GIT REMOTES
#https://gist.github.com/rvl/c3f156e117e22a25f242

git remote add github https://github.com/LennartAJansson/IntroAzure.git
git remote add azure https://dev.azure.com/LennartJansson/IoTSolutions/_git/IntroAzure

git remote set-url --add --push origin https://github.com/LennartAJansson/IntroAzure.git
git remote set-url --add --push origin https://dev.azure.com/LennartJansson/IoTSolutions/_git/IntroAzure

git remote show origin

