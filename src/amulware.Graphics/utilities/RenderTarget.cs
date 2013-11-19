using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace amulware.Graphics
{
    /// <summary>
    /// This class represents an OpenGL framebuffer object that can be rendered to.
    /// </summary>
    sealed public class RenderTarget : IDisposable
    {
        /// <summary>
        /// The handle of the OpenGL framebuffer object associated with this render target
        /// </summary>
        public readonly int Handle;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTarget"/> class.
        /// </summary>
        public RenderTarget()
        {
            int handle;
            GL.GenFramebuffers(1, out handle);
            this.Handle = handle;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTarget"/> class assigning a texture to its default color attachment.
        /// </summary>
        /// <param name="texture">The texture to attach.</param>
        public RenderTarget(Texture texture)
            : this()
        {
            this.Attach(FramebufferAttachment.ColorAttachment0, texture);
        }

        /// <summary>
        /// Attaches a <see cref="Texture"/> to the specified <see cref="FramebufferAttachment"/>, so it can be rendered to.
        /// </summary>
        /// <param name="attachment">The attachment.</param>
        /// <param name="texture">The texture.</param>
        public void Attach(FramebufferAttachment attachment, Texture texture, TextureTarget target = TextureTarget.Texture2D)
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, this.Handle);
            GL.FramebufferTexture2D(FramebufferTarget.DrawFramebuffer, attachment, target, texture, 0);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
        }

        /// <summary>
        /// Casts the <see cref="RenderTarget"/> to its OpenGL framebuffer object handle, for easy use with OpenGL functions.
        /// </summary>
        static public implicit operator int(RenderTarget rendertarget)
        {
            if (rendertarget == null)
                return 0;
            return rendertarget.Handle;
        }


        #region Disposing

        private bool disposed = false;

        public void Dispose()
        {
            this.dispose(true);
            GC.SuppressFinalize(this);
        }

        private void dispose(bool disposing)
        {
            if (this.disposed)
                return;

            int handle = this.Handle;
            GL.DeleteFramebuffers(1, ref handle);

            this.disposed = true;
        }

        ~RenderTarget()
        {
            this.dispose(false);
        }

        #endregion
    }
}
