#pragma once
#include <string>
#include "composedCommand.h"
#include "dockerUtils.h"
static  std::string start_docker_service()
{
    return "service docker start";
}



static  std::string un_tar(const std::string& path)
{
    return "tar xf " + path;
}

static  std::string un_tar_all_files_that_matches_into_folder(const std::string& pattern, const std::string& searchPath, const std::string& outPath)
{
    return "find " + searchPath + " -type f -name  \"" + pattern + "\" -exec tar xf {} -C " + outPath + " \\; ";
}

static  std::string change_dir(const std::string& path)
{
    return "cd " + path;
}

static  std::string erase_file(const std::string& name)
{
    return "rm " + name;
}
static  std::string create_directory_tree(const std::string& path)
{
    return "mkdir -p " + path;
}

static  std::string copy_file(const std::string& from, const std::string& to)
{
    return "cp " + from + " " + to;
}

static  std::string copy_dir(const std::string& from, const std::string& to)
{
    return "cp -r " + from + " " + to;
}
static  std::string move_file(const std::string& from, const std::string& to)
{
    return "mv " + from + " " + to;
}
static  std::string un_tar_rootfs_joined(const std::string& name, const std::string& untar_path)
{
    return "tar xfi " + name + " -C " + untar_path + "/   --same-owner --hard-dereference || :";
}

static  std::string tar_rootfs(const std::string& filename)
{
    return "tar cf " + filename +" * --same-owner --hard-dereference || :";
}


static  std::string erase_dir(const std::string&  path)
{
    return "chmod -R 777 " + path + "&&  rm -rf " + path;
}

static std::string echo(const std::string& txt)
{
    return "echo '" + txt + "'";
}

static std::string export_wsl_tar(const std::string& wsl_image_name, const std::string& tar_name)
{
    return "wsl.exe --export " + wsl_image_name + " " + tar_name;
}

static std::string docker_import(const std::string& tar_name, const dockerImage& dimage)
{
    return "docker import " + tar_name + " " + dimage.full_id;
}


static  std::string docker_pull(const dockerImage& dimage)
{
    return "docker pull " + dimage.full_id;
}

static std::string docker_container_create(const dockerImage& dimage, const std::string& tmpContainer)
{
    return "docker container create --name " + tmpContainer + " " + dimage.full_id;
}

static std::string docker_container_export(const std::string& container, const std::string& tar)
{
    return "docker export " + container + " -o " + tar;
}

static std::string docker_container_rm(const std::string& container)
{
    return "docker container rm " + container;

}
static  std::string docker_save(const dockerImage& dimage, const std::string& path)
{
    return "docker save " + dimage.full_id + " -o " + path;
}

static std::string docker_push(const dockerImage& dimage)
{
    return "docker push " + dimage.full_id;
}

static std::string docker_rmi()
{
    return "docker rmi -f $(docker images -q)";
}

static std::string docker_rmi(const dockerImage& dimage)
{
    return "docker rmi -f " + dimage.full_id;
}

ComposedCommand downloadDockerImage(const std::string& windowsPath, const std::string& temporaryLinuxPath, const std::string filename,  const dockerImage& dimage)
{
    ComposedCommand cmd;
    std::string extracted_path = temporaryLinuxPath + "/extracted";
    std::string session_id = std::to_string(session);
    auto toWslPath = [](std::string wp)
    {
        std::string append = "| sed -e 's|\\\\|/|g' -e 's|^\\([A-Za-z]\\)\\:/\\(.*\\)|/mnt/\\L\\1\\E/\\2|'";
        std::string ecp = "$(echo \"" +wp + std::string("\" ") + append + ")";
        return ecp;
    };

    cmd.addCommand(echo("creating temporary paths..."));
    cmd.addCommand(create_directory_tree(temporaryLinuxPath));
    cmd.addCommand(create_directory_tree(extracted_path));
    cmd.addCommand(change_dir(temporaryLinuxPath));
    cmd.addCommand(echo("Initializing docker..."));
    cmd.addCommand(start_docker_service());
    cmd.addCommand(docker_container_create(dimage, session_id));
    cmd.addCommand(docker_container_export(session_id, filename));
    cmd.addCommand(docker_container_rm(session_id));
    cmd.addCommand(move_file(filename, toWslPath(windowsPath)));
    cmd.addCommand(echo("Cleanup"));
    cmd.addCommand(erase_dir(temporaryLinuxPath));
    cmd.addCommand(docker_rmi(dimage));

    return cmd;
}

ComposedCommand wsl_to_docker(const std::string& temporaryLinuxPath, const std::string distro_name, const dockerImage& dimage)
{
        ComposedCommand cmd;
        std::string distro_name_tar = distro_name + ".tar.gz";
        cmd.addCommand(create_directory_tree(temporaryLinuxPath));
        cmd.addCommand(change_dir(temporaryLinuxPath));
        cmd.addCommand(start_docker_service());
        cmd.addCommand(export_wsl_tar(distro_name, distro_name_tar));
        cmd.addCommand(docker_import(distro_name_tar, dimage));
        cmd.addCommand(docker_push(dimage));
        cmd.addCommand(docker_rmi(dimage));
        cmd.addCommand(erase_dir(temporaryLinuxPath));
        return cmd;
}