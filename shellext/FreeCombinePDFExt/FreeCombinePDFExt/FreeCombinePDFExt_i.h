

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.00.0595 */
/* at Tue May 14 11:42:17 2019
 */
/* Compiler settings for FreeCombinePDFExt.idl:
    Oicf, W1, Zp8, env=Win32 (32b run), target_arch=X86 8.00.0595 
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
/* @@MIDL_FILE_HEADING(  ) */

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 475
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __FreeCombinePDFExt_i_h__
#define __FreeCombinePDFExt_i_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IFreeCombinePDFShellExt_FWD_DEFINED__
#define __IFreeCombinePDFShellExt_FWD_DEFINED__
typedef interface IFreeCombinePDFShellExt IFreeCombinePDFShellExt;

#endif 	/* __IFreeCombinePDFShellExt_FWD_DEFINED__ */


#ifndef __FreeCombinePDFShellExt_FWD_DEFINED__
#define __FreeCombinePDFShellExt_FWD_DEFINED__

#ifdef __cplusplus
typedef class FreeCombinePDFShellExt FreeCombinePDFShellExt;
#else
typedef struct FreeCombinePDFShellExt FreeCombinePDFShellExt;
#endif /* __cplusplus */

#endif 	/* __FreeCombinePDFShellExt_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


#ifndef __IFreeCombinePDFShellExt_INTERFACE_DEFINED__
#define __IFreeCombinePDFShellExt_INTERFACE_DEFINED__

/* interface IFreeCombinePDFShellExt */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IFreeCombinePDFShellExt;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("44FD9E69-EF6F-4F89-BC02-A5EEBA28ECAA")
    IFreeCombinePDFShellExt : public IUnknown
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct IFreeCombinePDFShellExtVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IFreeCombinePDFShellExt * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IFreeCombinePDFShellExt * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IFreeCombinePDFShellExt * This);
        
        END_INTERFACE
    } IFreeCombinePDFShellExtVtbl;

    interface IFreeCombinePDFShellExt
    {
        CONST_VTBL struct IFreeCombinePDFShellExtVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IFreeCombinePDFShellExt_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IFreeCombinePDFShellExt_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IFreeCombinePDFShellExt_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IFreeCombinePDFShellExt_INTERFACE_DEFINED__ */



#ifndef __FreeCombinePDFExtLib_LIBRARY_DEFINED__
#define __FreeCombinePDFExtLib_LIBRARY_DEFINED__

/* library FreeCombinePDFExtLib */
/* [helpstring][version][uuid] */ 


EXTERN_C const IID LIBID_FreeCombinePDFExtLib;

EXTERN_C const CLSID CLSID_FreeCombinePDFShellExt;

#ifdef __cplusplus

class DECLSPEC_UUID("7B95F48D-A040-472E-8732-438BBE08942F")
FreeCombinePDFShellExt;
#endif
#endif /* __FreeCombinePDFExtLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


