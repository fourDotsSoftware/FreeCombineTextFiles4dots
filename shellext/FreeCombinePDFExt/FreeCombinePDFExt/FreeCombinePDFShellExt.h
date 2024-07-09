// FreeCombinePDFShellExt.h : Declaration of the CFreeCombinePDFShellExt

#pragma once
#include "resource.h"       // main symbols
#include <sstream>
#include <vector>
#include "FreeCombinePDFExt_i.h"
#include <stdio.h>
#include <iostream>
using namespace std;
/*
//start
#include <shlobj.h>
#include <comdef.h>
//end
*/

#if defined(_WIN32_WCE) && !defined(_CE_DCOM) && !defined(_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA)
#error "Single-threaded COM objects are not properly supported on Windows CE platform, such as the Windows Mobile platforms that do not include full DCOM support. Define _CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA to force ATL to support creating single-thread COM object's and allow use of it's single-threaded COM object implementations. The threading model in your rgs file was set to 'Free' as that is the only threading model supported in non DCOM Windows CE platforms."
#endif



// CFreeCombinePDFShellExt

class ATL_NO_VTABLE CFreeCombinePDFShellExt :
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CFreeCombinePDFShellExt, &CLSID_FreeCombinePDFShellExt>,
	public IShellExtInit,
	public IContextMenu
	
	//public IFreeCombinePDFShellExt
{
public:
	CFreeCombinePDFShellExt();
	

DECLARE_REGISTRY_RESOURCEID(IDR_FreeCombinePDFSHELLEXT)

DECLARE_NOT_AGGREGATABLE(CFreeCombinePDFShellExt)

BEGIN_COM_MAP(CFreeCombinePDFShellExt)
	//COM_INTERFACE_ENTRY(IFreeCombinePDFShellExt)
	COM_INTERFACE_ENTRY(IShellExtInit)
	COM_INTERFACE_ENTRY(IContextMenu)
END_COM_MAP()	 	 

	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}

	void FinalRelease()
	{
	}

	protected:
  //TCHAR m_szFile[MAX_PATH];
  LPCWSTR m_szFile;
  string_list m_lsFiles;
  std::vector<LPCWSTR> m_lsFiles2;
  
public:
  // IShellExtInit

  STDMETHODIMP Initialize(LPCITEMIDLIST, LPDATAOBJECT, HKEY);
public:

	public:
  // IContextMenu

  STDMETHODIMP GetCommandString(UINT_PTR, UINT, UINT*, LPSTR, UINT);
  //STDMETHODIMP GetCommandString(UINT, UINT, UINT*, LPSTR, UINT);
  STDMETHODIMP InvokeCommand(LPCMINVOKECOMMANDINFO);
  STDMETHODIMP QueryContextMenu(HMENU, UINT, UINT, UINT, UINT);
    
  protected:
  HBITMAP     m_hSplitterVideoBmp;  
  
};


OBJECT_ENTRY_AUTO(__uuidof(FreeCombinePDFShellExt), CFreeCombinePDFShellExt)
