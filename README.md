# Welcome to WSLD!

This program was created with te intention of doing sharing and installing wsl machines more easily! <br>
It allows to just grab any dockerhub image and install it as a WSL subsystem or WSL2 Virtual machine.<br>

# Requirements

1.- WSL Installed, either 1 or 2 <br>
2.- A default WSL image (when doing bash) that has tar installed.<br>
# Usage

The usual command you want to do is:
``
wsld.exe -d <distroname> -i <dockerimage> 
`` 
**-d** and **-i** are the only required arguments. <br>
*distroname* will be the name which will be registered to WSL. <br>
*dockerimage* is the usual <repository/name:tag>, as if you were to do a docker pull.<br>

Some examples are:<br>
 ``
wsld.exe -d debian -i debian_d 
`` 
``
wsld.exe -d tianon/qemu -i qemu_d
`` 

Also, optionally, you can pass the version (1 for WSL1 and 2 for WSL2)<br>
or the installation directory of the WSL image.<br>

If there is no version, it will take the default for your wsl installation.<br>
~~~
  -o, --directory     Directory to install.

  -i, --image         Required. Docker Image to Install.

  -d, --distroname    Required. Name to give to the new distro

  -v, --version       Version for the new distro, the default is the wsl default, set 1 to WSL1, 2 to WSL2.

  --help              Display this help screen.

  --version           Display version information.
  ~~~


## Obtaining access to the installed image

The  usual "wsl" command can log into any installed distribution,<br>
so if we installed a debian image as "debian_d", we just need to do the following command: 
 
``
wsl -d debian_d
`` 

Also, check if the image has been installed with the command 

``
wsl -l -v
`` 
This command will show you all the installed distributions and its versions.
