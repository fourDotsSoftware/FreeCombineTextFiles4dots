#include "StdAfx.h"
#include "DirectoryHelper.h"

DirectoryHelper::DirectoryHelper(void)
{
}

DirectoryHelper::~DirectoryHelper(void)
{
}

bool DirectoryHelper::DirectoryExists(LPWSTR directoryName)
{
    DWORD attributes = GetFileAttributes(directoryName);
    if (attributes != INVALID_FILE_ATTRIBUTES && 
        attributes & FILE_ATTRIBUTE_DIRECTORY)
        return true;

    return false;
}

