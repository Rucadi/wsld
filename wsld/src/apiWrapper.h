#pragma once
#include <functional>
#include <Windows.h>
#include <wslapi.h>
#include <memory>
#include <iostream>
#include "composedCommand.h"
#include "options.h"
namespace Wsl
{

	const static bool IsDistributionRegistered(const std::wstring& view)
	{
		return WslIsDistributionRegistered(view.data());
	}
	const static bool IsDistributionRegistered(const std::string& str)
	{
		return IsDistributionRegistered(utf8_to_utf16(str));
	}

	const static bool RegisterDistribution(const std::wstring& distro, const std::wstring& tarpath)
	{
		return WslRegisterDistribution(distro.data(), tarpath.data()) == S_OK;
	}

	const static bool RegisterDistribution(const std::string& distro, const std::string& tarpath)
	{
		return RegisterDistribution(utf8_to_utf16(distro), utf8_to_utf16(tarpath));
	}


	const static bool Launch(const std::wstring& distro, const std::wstring& command)
	{
		HANDLE newProcess;
		if(getParsedOptions()["v"].count())
			WslLaunch(distro.data(), command.data(), false, GetStdHandle(0), GetStdHandle(STD_OUTPUT_HANDLE), GetStdHandle(STD_OUTPUT_HANDLE), &newProcess);
		else
			WslLaunch(distro.data(), command.data(), false, GetStdHandle(0), GetStdHandle(1), GetStdHandle(2), &newProcess);

		WaitForSingleObject(newProcess, INFINITE);
		CloseHandle(newProcess);
		return true;
	}

	const static bool Launch(const std::string& distro, const std::string& command)
	{
		return Launch(utf8_to_utf16(distro), utf8_to_utf16(command));
	}

	const static bool Launch(const std::string& distro, const std::wstring& command)
	{
		return Launch(utf8_to_utf16(distro), command);
	}

	const static bool Launch(const std::string& distro, ComposedCommand& cp)
	{
		auto cmd = cp.getCommandSequence();
		return Launch(distro, cmd);
	}
};