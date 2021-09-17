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
unsigned int  session = []() {    std::random_device rd; std::mt19937 mt(rd()); return mt(); }();

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

    std::string new_distro_name = getParsedOptions()["distro"].as<std::string>();
    std::string tarname = new_distro_name + std::to_string(session) + ".tar.gz";
    std::string imagename = getParsedOptions()["image"].as<std::string>();
    if (Wsl::IsDistributionRegistered(new_distro_name))
    {
        std::cerr << "A distro named: " << new_distro_name << " already exists" << std::endl;
        exit(1);
    }

    auto cmd = downloadDockerImage(getTempDirPath().string(), std::string("/tmp/wsld/") + std::to_string(session), tarname, dockerImage(imagename));
    Wsl::Launch("wsld", cmd);

    auto tarpath = getTempDirPath() /= tarname;
    auto userDefinedPath = std::filesystem::path(getParsedOptions()["onto"].as<std::string>() +"/"+ new_distro_name); 
    auto installpath = userDefinedPath==""? getProgramDataPath(new_distro_name) : userDefinedPath;
    importDistro(new_distro_name, tarpath, installpath);

}


void unistallWslImage()
{
    std::string command = "wsl --unregister " + getParsedOptions()["r"].as<std::string>();
    std::cout << command << std::endl;
    system(command.c_str());
}

static void dockerLogin(const std::string& username, const std::string& password)
{
    std::string command = "service docker start ||  docker login --username " + username + " --password " + password;
    Wsl::Launch("Wsld",command);
}


static void uploadWslToDockerhub()
{
    std::string distro_name = getParsedOptions()["distro"].as<std::string>();
    std::string tarname = distro_name + std::to_string(session) + ".tar.gz";
    std::string imagename = getParsedOptions()["image"].as<std::string>();


    if (!Wsl::IsDistributionRegistered(distro_name))
    {
        std::cerr << "A distro named: " << distro_name << " doesn't exists" << std::endl;
        exit(1);
    }

    std::cout << "wsl to docker" << std::endl;
    auto cmd = wsl_to_docker(std::string("/tmp/wsld/") + std::to_string(session), distro_name, dockerImage(imagename));
    Wsl::Launch("wsld", cmd);
    std::cout << "launched" << std::endl;
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

    auto isPresent = [&](std::string  str) {return getParsedOptions()[str].count(); };
    auto getStringOp = [&](std::string str) {return getParsedOptions()[str].as<std::string>(); };
    auto printHelp = [&]() {std::cout << options.help() << std::endl;; };
    if (isPresent("t"))
    {
        if (isPresent("d") && isPresent("i"))
            uploadWslToDockerhub();
        else printHelp();
    }
    else if (isPresent("l"))
    {
        if (isPresent("u") && isPresent("p"))
            dockerLogin(getStringOp("u"), getStringOp("p"));
        else printHelp();
    }
    else if (isPresent("r"))
    {
        unistallWslImage();
    }
    else if (isPresent("d") && isPresent("i"))
    {
        installDockerImageAsWsld();
    }
    else printHelp();
}
