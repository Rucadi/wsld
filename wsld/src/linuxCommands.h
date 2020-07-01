#pragma once
#include <string>
#include "composedCommand.h"
#include "dockerUtils.h"
static  std::string StartDockerService()
{
    return "service docker start";
}

static  std::string DockerPull(const std::string& rit)
{
    return "docker pull " + rit;
}

static  std::string DockerSave(const std::string& rit, const std::string& path)
{
    return "docker save " + rit + " -o " + path;
}

static  std::string UnTar(const std::string& path)
{
    return "tar xf " + path;
}

static  std::string UnTarAllFilesThatMatchesIntoFolder(const std::string& pattern, const std::string& searchPath, const std::string& outPath)
{
    return "find " + searchPath + " -type f -name  \"" + pattern + "\" -exec tar xf {} -C " + outPath + " \\; ";
}

static  std::string Change_directory(const std::string& path)
{
    return "cd " + path;
}

static  std::string EraseFile(const std::string& name)
{
    return "rm " + name;
}
static  std::string Create_directory_tree(const std::string& path)
{
    return "mkdir -p " + path;
}

static  std::string CopyFile(const std::string& from, const std::string& to)
{
    return "cp " + from + " " + to;
}

static  std::string CopyDir(const std::string& from, const std::string& to)
{
    return "cp -r " + from + " " + to;
}
static  std::string MoveFile(const std::string& from, const std::string& to)
{
    return "mv " + from + " " + to;
}
static  std::string Untar_rootfs_joined(const std::string& name, const std::string& untar_path)
{
    return "tar xfi " + name + " -C " + untar_path + "/   --same-owner --hard-dereference || :";
}

static  std::string Tar_rootfs(const std::string& filename)
{
    return "tar cf " + filename +" * --same-owner --hard-dereference || :";
}


static  std::string EraseDirectory(const std::string&  path)
{
    return "chmod -R 777 " + path + "&&  rm -rf " + path;
}

static std::string echo(const std::string& txt)
{
    return "echo '" + txt + "'";
}


ComposedCommand downloadDockerImage(const std::string& windowsPath, const std::string& temporaryLinuxPath, const std::string filename,  const dockerImage& dimage)
{
    ComposedCommand cmd;
    std::string extracted_path = temporaryLinuxPath + "/extracted";
    
    auto toWslPath = [](std::string wp)
    {
        std::string append = "| sed -e 's|\\\\|/|g' -e 's|^\\([A-Za-z]\\)\\:/\\(.*\\)|/mnt/\\L\\1\\E/\\2|'";
        std::string ecp = "$(echo \"" +wp + std::string("\" ") + append + ")";
        return ecp;
    };

    cmd.addCommand(echo("creating temporary paths..."));
    cmd.addCommand(Create_directory_tree(temporaryLinuxPath));
    cmd.addCommand(Create_directory_tree(extracted_path));
    cmd.addCommand(Change_directory(temporaryLinuxPath));
    cmd.addCommand(echo("Initializing docker..."));
    cmd.addCommand(StartDockerService());
    cmd.addCommand(DockerPull(dimage.full_id));
    cmd.addCommand(DockerSave(dimage.full_id, "docker"));
    cmd.addCommand(echo("Decompressing docker images..."));
    cmd.addCommand(UnTar("docker"));
    cmd.addCommand(UnTarAllFilesThatMatchesIntoFolder("*tar", temporaryLinuxPath, temporaryLinuxPath+"/extracted"));
    cmd.addCommand(Change_directory(extracted_path));
    cmd.addCommand(echo("Creating WSL TAR"));
    cmd.addCommand(Tar_rootfs(filename));
    cmd.addCommand(echo("Moving WSL TAR into Windows FS: "+windowsPath));
    cmd.addCommand(MoveFile(filename, toWslPath(windowsPath)));
    cmd.addCommand(echo("Cleanup"));
    cmd.addCommand(EraseDirectory(temporaryLinuxPath));

    return cmd;
}