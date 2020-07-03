#pragma once
#include "external/cxxopts.hpp"
#include <filesystem>
#include <vector>
#include <string>

static cxxopts::Options options = []()
{
    cxxopts::Options opp("Wsld", "Install docker images as wsl images on windows");
    opp.add_options()
        ("d,distro", "Name to give the new distro", cxxopts::value<std::string>())
        ("i,image", "Docker Image name", cxxopts::value<std::string>())
        ("r,remove", "Distro name to remove", cxxopts::value<std::string>())
        ("l,login", "Try to login docker")
        ("u,user", "Docker username", cxxopts::value<std::string>())
        ("p,password", "Docker password", cxxopts::value<std::string>())
        ("t,transfer", "Uploads an wsl distribution to dockerhub, you need to be signed in")
        ("v,verbose", "Verbose output", cxxopts::value<bool>()->default_value("false"));
    return opp;
}();

extern unsigned int  session;

static cxxopts::ParseResult& getParsedOptions(int argc = 0, char** argv = nullptr)
{
    static auto poptions = options.parse(argc, argv);
    return poptions;
}


static std::filesystem::path getProgramDataPath(std::string distro_name)
{
    auto path = std::filesystem::temp_directory_path()
        .parent_path()
        .parent_path();

    path /= "Roaming";
    path /= "wsld";
    path /= distro_name;

    if (!std::filesystem::exists(path))
        std::filesystem::create_directories(path);
    return path;
}

static  std::filesystem::path getTempDirPath()
{
    auto path = std::filesystem::temp_directory_path().append("wsld");
    if (!std::filesystem::exists(path))
        std::filesystem::create_directories(path);
    return path;
}



static std::wstring utf8_to_utf16(const std::string& utf8)
{
    std::vector<unsigned long> unicode;
    size_t i = 0;
    while (i < utf8.size())
    {
        unsigned long uni;
        size_t todo;
        unsigned char ch = utf8[i++];
        if (ch <= 0x7F)
        {
            uni = ch;
            todo = 0;
        }
        else if (ch <= 0xBF)
        {
            return L"";
        }
        else if (ch <= 0xDF)
        {
            uni = ch & 0x1F;
            todo = 1;
        }
        else if (ch <= 0xEF)
        {
            uni = ch & 0x0F;
            todo = 2;
        }
        else if (ch <= 0xF7)
        {
            uni = ch & 0x07;
            todo = 3;
        }
        else
        {
            return L"";
        }
        for (size_t j = 0; j < todo; ++j)
        {
            if (i == utf8.size())
                return L"";
            unsigned char ch = utf8[i++];
            if (ch < 0x80 || ch > 0xBF)
                return L"";
            uni <<= 6;
            uni += ch & 0x3F;
        }
        if (uni >= 0xD800 && uni <= 0xDFFF)
            return L"";
        if (uni > 0x10FFFF)
            return L"";
        unicode.push_back(uni);
    }
    std::wstring utf16;
    for (size_t i = 0; i < unicode.size(); ++i)
    {
        unsigned long uni = unicode[i];
        if (uni <= 0xFFFF)
        {
            utf16 += (wchar_t)uni;
        }
        else
        {
            uni -= 0x10000;
            utf16 += (wchar_t)((uni >> 10) + 0xD800);
            utf16 += (wchar_t)((uni & 0x3FF) + 0xDC00);
        }
    }
    return utf16;
}