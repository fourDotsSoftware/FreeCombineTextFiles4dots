// dllmain.h : Declaration of module class.

class CFreeCombinePDFExtModule : public CAtlDllModuleT< CFreeCombinePDFExtModule >
{
public :
	DECLARE_LIBID(LIBID_FreeCombinePDFExtLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_HANDYRESIZEREXT, "{8394A8A8-3820-4121-A4FC-9CCD3780365E}")
};

extern class CFreeCombinePDFExtModule _AtlModule;
