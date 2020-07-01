#pragma once
#include <vector>
#include <string>
#include "options.h"
class ComposedCommand
{
	std::vector<std::wstring> commands;

public:
	
	void clear() 
	{
		commands.clear();
	}

	void addCommand(const std::wstring& command)
	{
		commands.push_back(command);
	}
	void addCommand(const std::string& command)
	{
		addCommand(utf8_to_utf16(command));
	}

	std::wstring getCommandSequence()
	{
		if (commands.size() == 0) return L"";

		std::wstring str = commands[0];
		for (int i = 1; i < commands.size(); ++i)
			str += L" ; " + commands[i];
		
		return str;
	}


};