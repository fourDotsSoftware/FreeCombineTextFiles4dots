// FreeCombinePDFShellExt.cpp : Implementation of CFreeCombinePDFShellExt

#include "stdafx.h"
#include "FreeCombinePDFShellExt.h"
#include <atlconv.h>  // for ATL string conversion macros
#include "Resource.h"
#include <atlcom.h>
#include <atlstr.h>
#include <stdio.h>
#include <tchar.h>
#define BUFSIZE 65536
#include <cwchar> 
#include <iostream>
#include <string>
#include <Algorithm>
#include "DirectoryHelper.h"

//#include "ShobjIdl.h"
//#include "Shellapi.h"

// CFreeCombinePDFShellExt
CFreeCombinePDFShellExt::CFreeCombinePDFShellExt()
{
	m_hSplitterVideoBmp = LoadBitmap( _AtlBaseModule.GetModuleInstance()  ,
                           MAKEINTRESOURCE(IDB_BITMAP1) );
	
}


STDMETHODIMP CFreeCombinePDFShellExt::Initialize ( 
  LPCITEMIDLIST pidlFolder,
  LPDATAOBJECT pDataObj,
  HKEY hProgID )
{
FORMATETC fmt = { CF_HDROP, NULL, DVASPECT_CONTENT, -1, TYMED_HGLOBAL };
STGMEDIUM stg = { TYMED_HGLOBAL };
HDROP     hDrop;

    // Look for CF_HDROP data in the data object.
    if ( FAILED( pDataObj->GetData ( &fmt, &stg ) ))
        {
        // Nope! Return an "invalid argument" error back to Explorer.
        return E_INVALIDARG;
        }

    // Get a pointer to the actual data.
    hDrop = (HDROP) GlobalLock ( stg.hGlobal );

    // Make sure it worked.
    if ( NULL == hDrop )
        return E_INVALIDARG;

    // Sanity check - make sure there is at least one filename.
UINT uNumFiles = DragQueryFile ( hDrop, 0xFFFFFFFF, NULL, 0 );
HRESULT hr = S_OK;

    if ( 0 == uNumFiles )
        {
        GlobalUnlock ( stg.hGlobal );
        ReleaseStgMedium ( &stg );
        return E_INVALIDARG;
        }

	bool isok=false;

	std::vector<LPCWSTR>::iterator it;
	it = m_lsFiles2.begin();		

	bool isfiletype=false;
	std::wstring filetype=L".RAR";
	std::wstring filetype_exe=L".EXE";

	std::wstring filetypes[]= {				

		L".PDF"
	};			

	int ftUpperBound=sizeof(filetypes)/sizeof(std::wstring);

	DirectoryHelper dirh;

	for (int k=0;k<uNumFiles;k++)
	{
		LPWSTR m_szFile3=new WCHAR[MAX_PATH];
		bool isfiletype=false;

		if ( 0 != DragQueryFile ( hDrop, k, (LPWSTR) m_szFile3, MAX_PATH ))
		{						
			if (!dirh.DirectoryExists((LPWSTR)m_szFile3))
			{
				// if it does not end with .PDF do not show menu item
				std::wstring shfile(m_szFile3);
				std::transform(shfile.begin(), shfile.end(),shfile.begin(), ::toupper);
				
				for (int m=0;m<ftUpperBound;m++)
				{				
					if (shfile.length() >= filetypes[m].length())
					{			
						if (0 == shfile.compare (shfile.length() - filetypes[m].length(), filetypes[m].length(), filetypes[m]))
						{
							isfiletype=true;
							break;
						}
					}				
				}						
			}
			else
			{
				isfiletype=true;
			}

			if (isfiletype)
			{
				isok=true;
				it=m_lsFiles2.insert ( it , m_szFile3 );
			//m_lsFiles2.a.push_back(m_szFile);
			}		
		}
	}
    // Get the name of the first file and store it in our member variable m_szFile.
    //if ( 0 == DragQueryFile ( hDrop, 0, m_szFile, MAX_PATH ) )
    //hr = E_INVALIDARG;
	/*
	if (!isfiletype)
	{
		return E_INVALIDARG;
	}
	*/

	if (!isok)
	{
		hr = E_INVALIDARG;
	}

    GlobalUnlock ( stg.hGlobal );
    ReleaseStgMedium ( &stg );

    return hr;
}

HRESULT CFreeCombinePDFShellExt::QueryContextMenu (
  HMENU hmenu, UINT uMenuIndex, UINT uidFirstCmd,
  UINT uidLastCmd, UINT uFlags )
{
  // If the flags include CMF_DEFAULTONLY then we shouldn't do anything.

  if ( uFlags & CMF_DEFAULTONLY )
    return MAKE_HRESULT ( SEVERITY_SUCCESS, FACILITY_NULL, 0 );

	UINT uID = uidFirstCmd;
	UINT upos=uMenuIndex;
			
        CRegKey reg;
        LONG    lRet;

        lRet = reg.Open ( HKEY_CURRENT_USER,
                          _T("Software\\softpcapps Software\\Free Combine Text 4dots"),KEY_QUERY_VALUE);

		WCHAR szFp[MAX_PATH];		

		bool suc=true;

		if (lRet!=ERROR_SUCCESS)
		{
			suc=false;			
		}
		
		DWORD dwSizeFp = sizeof(szFp) / sizeof(WCHAR); // # of characters in the buffer 
		
		lRet=reg.QueryStringValue(_T("Menu Item Caption"),szFp,&dwSizeFp);
 
		if (lRet!=ERROR_SUCCESS)
		{
			suc=false;			
		}

		if (suc)
		{
			InsertMenu ( hmenu, upos++, MF_BYPOSITION, uID++, szFp);				
		}
		else
		{
			InsertMenu ( hmenu, upos++, MF_BYPOSITION, uID++, L"Combine PDF");				
		}							

	if ( NULL != m_hSplitterVideoBmp) 
    SetMenuItemBitmaps ( hmenu, upos-1, MF_BYPOSITION, m_hSplitterVideoBmp, m_hSplitterVideoBmp );		

    return MAKE_HRESULT ( SEVERITY_SUCCESS, FACILITY_NULL, uID - uidFirstCmd );
	
	//return MAKE_HRESULT ( SEVERITY_SUCCESS, FACILITY_NULL, 1 );
}


HRESULT CFreeCombinePDFShellExt::GetCommandString (
  UINT_PTR idCmd, UINT uFlags, UINT* pwReserved,
//  UINT idCmd, UINT uFlags, UINT* pwReserved,
  LPSTR pszName, UINT cchMax )

{
USES_CONVERSION;		

return E_INVALIDARG;
  // Check idCmd, it must be 0 since we have only one menu item.

 // if ( 0 != idCmd )
  //  return E_INVALIDARG;
//	return E_FAIL;
 
  // If Explorer is asking for a help string, copy our string into the

  // supplied buffer.
  
  if (uFlags==GCS_VALIDATE)
  {
	return NOERROR;
  }
  else if ( uFlags == GCS_HELPTEXT || uFlags==GCS_HELPTEXTW)
    {
    LPCTSTR szText = _T("Combine PDF");	

	//HINSTANCE hInst = AfxGetInstanceHandle();
    if ( uFlags & GCS_UNICODE )
      {
      // We need to cast pszName to a Unicode string, and then use the

      // Unicode string copy API.

      lstrcpynW ( (LPWSTR) pszName, T2CW(szText), cchMax );
      }
    else
      {
      // Use the ANSI string copy API to return the help string.

      lstrcpynA ( pszName, T2CA(szText), cchMax );
      }
 
    return S_OK;
	//return NOERROR;
    }
	else if ( uFlags == GCS_VERB || uFlags==GCS_VERBW)
    {
	//	MessageBox ( NULL , _T("gcs_verb"), _T("SimpleShlExt"),MB_ICONINFORMATION );
    LPCTSTR szText = _T("Combine PDF");
 
    if ( uFlags & GCS_UNICODE )
      {
      // We need to cast pszName to a Unicode string, and then use the

      // Unicode string copy API.

      lstrcpynW ( (LPWSTR) pszName, T2CW(szText), cchMax );
      }
    else
      {
      // Use the ANSI string copy API to return the help string.

      lstrcpynA ( pszName, T2CA(szText), cchMax );
      }
 
    return S_OK;
	//return NOERROR;
    }
 
   
  //return E_INVALIDARG;
  return S_OK;
}

HRESULT CFreeCombinePDFShellExt::InvokeCommand(
  LPCMINVOKECOMMANDINFO pCmdInfo)
{
  // If lpVerb really points to a string, ignore this function call and bail out.
/*
	BOOL fEx = FALSE;
    BOOL fUnicode = FALSE;

    if(pCmdInfo->cbSize == sizeof(CMINVOKECOMMANDINFOEX))
    {
        fEx = TRUE;
        if((lpcmi->fMask & CMIC_MASK_UNICODE))
        {
            fUnicode = TRUE;
        }
    }

    if( !fUnicode && HIWORD(pCmdInfo->lpVerb))
    {
		  if ( 0 != HIWORD( pCmdInfo->lpVerb) )
			return E_INVALIDARG;
		/*
        if(StrCmpIA(lpcmi->lpVerb, m_pszVerb))
        {
            return E_FAIL;
        }*/
   // }
/*
    else if( fUnicode && HIWORD(((CMINVOKECOMMANDINFOEX *) pCmdInfo)->lpVerbW))
    {
        if(StrCmpIW(((CMINVOKECOMMANDINFOEX *)lpcmi)->lpVerbW, m_pwszVerb))
        {
            return E_FAIL;
        }
    }

    else if(LOWORD(lpcmi->lpVerb) != IDM_DISPLAY)
    {
        return E_FAIL;
    }
*/
	//MessageBox ( NULL ,_T("Invokecmd0"), _T("SimpleShlExt"),MB_ICONINFORMATION );

  if ( 0 != HIWORD( pCmdInfo->lpVerb) )
    return E_INVALIDARG;
 
  //MessageBox ( NULL ,_T("Invokecmd"), _T("SimpleShlExt"),MB_ICONINFORMATION );

	HANDLE hFile;
	HANDLE hTempFile;
	DWORD dwRetVal;
	DWORD dwBytesRead;
	DWORD dwBytesWritten;
	DWORD dwBufSize = BUFSIZE;
	UINT uRetVal;
	WCHAR szTempName[MAX_PATH] = TEXT("just_dummy");
	//LPWSTR buffer = malloc(sizeof(BUFSIZE));
	WCHAR lpPathBuffer[MAX_PATH] = TEXT("just_dummy");
	BOOL fSuccess;
	WCHAR szTempFileName[MAX_PATH];  

  // Get the command index - the only valid one is 0.  		
	
// Get the temp path.
dwRetVal = GetTempPath(dwBufSize, // length of the buffer
lpPathBuffer); // buffer for path
if (dwRetVal > dwBufSize)
{
return E_INVALIDARG;
}
else
{	
	uRetVal = GetTempFileName(lpPathBuffer, // directory for tmp files
                              TEXT("CWD"),     // temp file name prefix 
                              0,                // create unique name 
                              szTempFileName);  // buffer for name 
	
	if (uRetVal == 0)
	{
		return E_INVALIDARG;
	}

	//lstrcatW(lpPathBuffer,_T("Free Combine Text 4dots.txt"));
	

//MessageBox ( NULL , lpPathBuffer, _T("SimpleShlExt"),
 //                  MB_ICONINFORMATION );


}

/*
// Create a temporary file.
uRetVal = GetTempFileName(lpPathBuffer, // directory for tmp files
L"NEW", // temp file name prefix
0, // create unique name
szTempName); // buffer for the name

MessageBox ( NULL , szTempName, _T("SimpleShlExt"),
                   MB_ICONINFORMATION );


if (uRetVal == 0)
{
return E_INVALIDARG;
}

MessageBox ( NULL , _T("TEMPFILEGET"), _T("SimpleShlExt"),
                   MB_ICONINFORMATION );
*/
// Create the new file to write the upper-case version to.
//1hTempFile = CreateFile((LPTSTR) lpPathBuffer, // file name
hTempFile = CreateFile((LPTSTR) szTempFileName, // file name
GENERIC_READ | GENERIC_WRITE, // open r-w
0, // do not share
NULL, // default security
CREATE_ALWAYS, // overwrite existing
FILE_ATTRIBUTE_NORMAL,// normal file
NULL); // no template
 

//MessageBox ( NULL , _T("TEMPFILECREATED"), _T("SimpleShlExt"),MB_ICONINFORMATION );

if (hTempFile == INVALID_HANDLE_VALUE)
{
	return E_INVALIDARG;
}

  int iVerb=LOWORD(pCmdInfo->lpVerb);

   WCHAR sParam[40000];
   lstrcpyW(sParam,_T(" "));
/*
   lstrcpyW(sParam,_T("-Imgs:"));
   lstrcpyW(sParam,_T("\""));
   lstrcatW(sParam,m_szFile);
   lstrcatW(sParam,_T("\""));
*/

  for (int k=0;k<m_lsFiles2.size();k++)
  {
	//1
	//MessageBox ( NULL , (LPCWSTR)m_lsFiles2[k], _T("SimpleShlExt"),MB_ICONINFORMATION );

   lstrcatW(sParam,_T("\""));
   lstrcatW(sParam,(WCHAR*)m_lsFiles2[k]);
   lstrcatW(sParam,_T("\" "));
  }

  bool only_one_file=true;

  if (m_lsFiles2.size()>1)
  {
	only_one_file=false;
  } 

 // lstrcatW(sParam,_T(" -Cmd:"));

  //  switch ( LOWORD( pCmdInfo->lpVerb ) )

  WCHAR cmd[100];

  //MessageBox ( NULL ,cstr , _T("SimpleShlExt"),MB_ICONINFORMATION );	    

//lstrcatW(sParam,cmd);
      
	//MessageBox ( NULL , sParam, _T("SimpleShlExt"),MB_ICONINFORMATION );	    

		WCHAR szMsg[MAX_PATH + 32];
	    CRegKey reg;
        LONG    lRet;

        lRet = reg.Open ( HKEY_LOCAL_MACHINE,
                          _T("Software\\softpcapps Software\\Free Combine Text 4dots"),KEY_QUERY_VALUE);

		//1
		//MessageBox ( NULL , _T("test"), _T("SimpleShlExt"),MB_ICONINFORMATION );	    

		if (lRet!=ERROR_SUCCESS)
		{
			return E_UNEXPECTED;
		}

		TCHAR szFp[MAX_PATH];		
		DWORD dwSizeFp = sizeof(szFp) / sizeof(TCHAR); // # of characters in the buffer 
		
		lRet=reg.QueryStringValue(_T("InstallationDirectory"),szFp,&dwSizeFp);
 
		if (lRet!=ERROR_SUCCESS)
		{
			return E_UNEXPECTED;
		}

		//1
		//MessageBox ( NULL , _T("test2"), _T("SimpleShlExt"),MB_ICONINFORMATION );	    

		
		//MessageBox ( pCmdInfo->hwnd, m_szFile, _T("SimpleShlExt"),
          //         MB_ICONINFORMATION );


		//ShellExecute(NULL, _T("open"), _T("test.txt"), NULL, _T("."),SW_SHOWNORMAL);
		TCHAR szFp2[MAX_PATH];
		//szFp=lstrcat(szFp,_T("ImgTransformer.exe"));
		
		wcscat_s(szFp,_T("\\FreeCombinePDF.exe") );

		//MessageBox ( pCmdInfo->hwnd, szFp, _T("SimpleShlExt"),
          //         MB_ICONINFORMATION );		
		
		//28.9.11 lstrcpyW(szMsg,_T("-showunlockfile "));
		
		lstrcpyW(szFp2,_T("\""));
		lstrcatW(szFp2,szFp);
		lstrcatW(szFp2,_T("\""));
		
		WCHAR szTempParam[MAX_PATH];

		if (iVerb==0)
		{
			lstrcpyW(szTempParam,_T("-TempFile:\""));
		}
		
		/*
		else if (iVerb==1)
		{
			lstrcpyW(szTempParam,_T("-unlock -TempFile:\""));
		}
		else if (iVerb==2)
		{
			lstrcpyW(szTempParam,_T("-unlockopen -TempFile:\""));
		}
		*/
		//1lstrcatW(szTempParam,lpPathBuffer);

		lstrcatW(szTempParam,szTempFileName);
		lstrcatW(szTempParam,_T("\""));

		//wcscat_s(szMsg,_T("-showunlockfile "));
		//wcscat_s(szMsg,m_szFile);

		//wsprintf ( szMsg, _T("-showunlockfile "), m_szFile );
		//szMsg=lstrcat(_T("-showunlockfile "),m_szFile);
				
		//MessageBox ( pCmdInfo->hwnd, szMsg, _T("SimpleShlExt"),MB_ICONINFORMATION );

		//1ShellExecute(NULL, _T("open"), szFp, szMsg, _T("."), SW_SHOWNORMAL);


		//MessageBox ( NULL , sParam, _T("SimpleShlExt"),
       //          MB_ICONINFORMATION );	    

		//MessageBox ( NULL , szFp2, _T("SimpleShlExt"),
        //         MB_ICONINFORMATION );	    

	//	ShellExecute(NULL, _T("open"), szFp2, sParam, _T("."), SW_SHOWNORMAL);

		
	//MessageBox ( pCmdInfo->hwnd, szTempName, _T("SimpleShlExt"),
	//                   MB_ICONINFORMATION );
		
		//MessageBox ( NULL, _T("OK1"), _T("SimpleShlExt"),MB_ICONINFORMATION );

		fSuccess = WriteFile(hTempFile,
		sParam,
		std::wcslen (sParam)*sizeof(wchar_t),
		&dwBytesWritten,
		NULL);

		//1
		//MessageBox ( NULL, _T("OK2"), _T("SimpleShlExt"),MB_ICONINFORMATION );

		CloseHandle (hTempFile);

		//MessageBox ( NULL , szTempParam, _T("SimpleShlExt"),MB_ICONINFORMATION );	    

		//1
		//MessageBox ( NULL, _T("OK3"), _T("SimpleShlExt"),MB_ICONINFORMATION );
		if (!fSuccess)
		{
			return E_INVALIDARG;
		}

		//removeShellExecute(NULL, _T("open"), szFp2, sParam, _T("."), SW_SHOWNORMAL);
        ShellExecute(NULL, _T("open"), szFp2, szTempParam, _T("."), SW_SHOWNORMAL);

		//wsprintf ( szMsg, _T("The selected file was:\n\n%s"), m_szFile );
 
		//1
		//MessageBox ( NULL, _T("OK"), _T("SimpleShlExt"),MB_ICONINFORMATION );
 
      return S_OK;
   }