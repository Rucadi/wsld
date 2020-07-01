#pragma once

#include <string>
struct dockerImage
{
    std::string full_id;
    std::string repository, distribution, tag;
    dockerImage(std::string rdt)
    {
        const static auto getRepository = [](const std::string& str)
        {
            return str.find_first_of('/') == -1 ? "library" : str.substr(0, str.find_first_of('/'));
        };
        const static auto getTag = [](const std::string& str)
        {
            return str.find_first_of(':') == -1 ? "latest" : str.substr(str.find_first_of(':') + 1, str.size());
        };
        const static auto getDistribution = [](const std::string& str)
        {
            int idx = str.find_first_of('/');
            int idc = str.find_first_of(':');
            int left = idx == -1 ? 0 : idx + 1;
            int right = idc == -1 ? str.size() : idc - left;
            return str.substr(left, right);
        };
        repository = getRepository(rdt);
        distribution = getDistribution(rdt);
        tag = getTag(rdt);

        full_id = repository + '/' + distribution + ':' + tag;

    }
    
};
