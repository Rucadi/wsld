# Welcome to WSLD!

This program was created with the intention of sharing and installing WSL machines more easily! <br>
It allows you to grab any dockerhub image and install it as a WSL subsystem or WSL2 Virtual machine.<br>



To login docker you can use:  
``
wsld.exe docker -l -u user -p password
``  
this allows you to install private repos!



# Requirements

1. WSL 2 Installed  
2. A default WSL image

# Usage

The usual command you want to do is:  
``
wsld.exe -d <distroname> -i <dockerimage> 
``  
**-d** and **-i** are the only required arguments.  
*distroname* is the name which will be registered to WSL.  
*dockerimage* is the usual <repository/name:tag>, as if you were to do a docker pull.  

Some examples are:  
``
wsld.exe -d debian_d -i debian
``  
``
wsld.exe -d qemu_d -i tianon/qemu
``  
~~~
  wsld [OPTION...]

  -d, --distro arg    Name to give the new distro
  -i, --image arg     Docker Image name
  -r, --remove arg    Distro name to remove
  -l, --login         Try to login docker
  -u, --user arg      Docker username
  -p, --password arg  Docker password
  -v, --verbose       Verbose output
  -t, --transfer      if you logged in into docker, you can upload an wsl image to docker using -d -i as inputs

~~~

## Obtaining access to the installed image

The usual "wsl" command can log into any installed distribution,  
so if we installed a debian image as "debian_d", we just need to do the following command:  
``
wsl -d debian_d
``  

Also, check if the image has been installed with the command:  
``
wsl -l -v
``  

This command will show you all the installed distributions and its versions.

This program is free to use, but if you want to invite me to a coffee, feel free :)  
<img src="https://logos-download.com/wp-content/uploads/2016/03/Pay_Pal_logotype_logo_emblem_2.png" width="100" height="100">  
https://www.paypal.me/rucadi


I plan on rewriting the code in a near future. 
