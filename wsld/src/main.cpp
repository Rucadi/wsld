// ClangTest.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include "apiWrapper.h"
#include <string_view>
#include "external/cxxopts.hpp"
#include <filesystem>
#include "linuxCommands.h"
#include <random>
#include <cmath>
#include "options.h"
#include<Windows.h>

#pragma comment(lib, "urlmon.lib")


void importDistro(std::string name, std::filesystem::path tarDir, std::filesystem::path installDir, int version=2, bool removeTar = true)
{
    std::string importCommand = "wsl --import ";
    importCommand += name + " ";
    importCommand += installDir.string()+" ";
    importCommand += tarDir.string()+" ";
    importCommand += "--version " + std::to_string(version);
    system(importCommand.c_str());
    if (removeTar) std::filesystem::remove(tarDir);

}

void installDockerImageAsWsld()
{
    unsigned int  session = rand();

    std::string new_distro_name = getParsedOptions()["distro"].as<std::string>();
    std::string tarname = new_distro_name + std::to_string(session) + ".tar.gz";
    std::string imagename = getParsedOptions()["image"].as<std::string>();

    if (Wsl::IsDistributionRegistered(new_distro_name))
    {
        std::cerr << "A distro named: " << new_distro_name << " already exists" << std::endl;
        exit(1);
    }

    auto cmd = downloadDockerImage(getTempDirPath().string(), "/tmp/wsld", tarname, dockerImage(imagename));
    Wsl::Launch("wsld", cmd);

    auto tarpath = getTempDirPath() /= tarname;
    auto installpath = getProgramDataPath(new_distro_name);
    importDistro(new_distro_name, tarpath, installpath);
}


void unistallWslImage(const std::string& name)
{
    std::string command = "wsl --unregister " + name;
    std::cout << command << std::endl;
    system(command.c_str());
}

static void dockerLogin(const std::string& username, const std::string& password)
{
    std::string command = "service docker start ||  docker login --username " + username + " --password " + password;
    Wsl::Launch("Wsld",command);
}



void installWsldImage()
{
    auto tarpath = getTempDirPath() /= "wsld.tar";
    auto installpath = getProgramDataPath("wsld");
    URLDownloadToFile(NULL, L"https://gitlab.com/ruben.cano96/wsld_image/-/raw/master/wsld.tar", tarpath.wstring().c_str(), 0, NULL);
    importDistro("wsld", tarpath, installpath);
}
int main(int argc, char** argv)
{
    getParsedOptions(argc, argv);

    if (!Wsl::IsDistributionRegistered("wsld"))
    {
        std::cout << "Installing WSLD image... " << std::endl;
        std::cout << "It can take few minutes depending on your internet connection" << std::endl;
        std::cout << "This step is only needed once" << std::endl;
        installWsldImage();
    }

    if (getParsedOptions()["l"].count())
    {
        if (getParsedOptions()["u"].count() && getParsedOptions()["p"].count())
        {
            dockerLogin(getParsedOptions()["u"].as<std::string>(), getParsedOptions()["p"].as<std::string>());
        }
    }
    if (getParsedOptions()["r"].count())
    {
        unistallWslImage(getParsedOptions()["r"].as<std::string>());
    }
    else if (getParsedOptions()["distro"].count() && getParsedOptions()["image"].count())
    {
        installDockerImageAsWsld();
    }
    else std::cout << options.help() << std::endl;;
}
