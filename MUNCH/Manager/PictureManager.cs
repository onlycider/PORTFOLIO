using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureManager
{
    public static Texture2D TakePicture(int maxSize)
    {
        Texture2D texture = null;

        NativeCamera.Permission permission = NativeCamera.TakePicture( ( path ) =>
        {
            Debug.Log( "Image path: " + path );
            if( path != null )
            {
                // Create a Texture2D from the captured image
                texture = NativeCamera.LoadImageAtPath( path, maxSize );
                if( texture == null )
                {
                    Debug.Log( "Couldn't load texture from " + path );
                    return;
                }
            }
        }, maxSize );

        return texture;
    }

    public static Texture2D PickPictureToLocalStorage(int maxSize)
    {
        Texture2D texture = null;

        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery( ( path ) =>
        {
            Debug.Log( "Image path: " + path );
            if( path != null )
            {
                // Create Texture from selected image
                texture = NativeGallery.LoadImageAtPath( path, maxSize );
                if( texture == null )
                {
                    Debug.Log( "Couldn't load texture from " + path );
                    return;
                }
            }
        } );

        Debug.Log( "Permission result: " + permission );
        return texture;
    }
}
