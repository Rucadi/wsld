# Welcome to WSLD!

This program was created with the intention of sharing and installing WSL machines more easily! <br>
It allows you to grab any dockerhub image and install it as a WSL subsystem or WSL2 Virtual machine.<br>

# New

Now it accepts uploading images to dockerhub too!!  

Give it a try:  

This next command will upload the installed distro "debian" into my dockerhub repository rucadi.  
``
wsld.exe docker upload -d debian -i rucadi/debian:latest
``  

To login you can use:  
``
wsld.exe docker login -u user -p password
``  

This accepts no parameters, in that case, you must pass them iteratively.

If you try to use a command that requires login, you will be prompted with the login dialog.

# Build your own Dockerfiles!

With this feature, you can build you own images for WSL from Dockerfiles.  
``
wsld.exe docker build -d wsl_distro_name
``  

There are more parameters! check the help!  

# Example
Go into asciinema to see how it works!  
https://asciinema.org/a/EaGqIVG9IbWJ6iSw70Sl7eQha  

# Requirements

1. WSL Installed, either 1 or 2.  
2. A default WSL image (when doing bash) that has tar installed.
3. Tested in windows build 18922, you need an upgraded "wsl" to work. If you do `wsl -l -v` and it works, you are good to go.

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

Also, optionally, you can pass the version (1 for WSL1 and 2 for WSL2) or the installation directory of the WSL image.  

If there is no version, it will take the default for your wsl installation.  
~~~
  -o, --directory     Directory to install.

  -i, --image         Required. Docker Image to Install.

  -d, --distroname    Required. Name to give to the new distro.

  -v, --version       Version for the new distro, the default is the wsl default, set 1 to WSL1, 2 to WSL2.

  --help              Display this help screen.

  --version           Display version information.
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
