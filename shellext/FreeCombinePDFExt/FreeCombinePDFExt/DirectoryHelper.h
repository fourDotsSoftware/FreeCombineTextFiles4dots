#pragma once

class DirectoryHelper
{
public:
	DirectoryHelper(void);
	~DirectoryHelper(void);

	bool DirectoryExists(LPWSTR);
};
