﻿using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using Vlc.DotNet.Core.Rincewind;
using Vlc.DotNet.Forms.TypeEditors;

namespace Vlc.DotNet.Forms
{
    public partial class VlcRincewindControl : Control, ISupportInitialize
    {
        private readonly object myEventSyncLocker = new object();
        private VlcMediaPlayer myVlcMediaPlayer;

        [Editor(typeof(DirectoryEditor), typeof(UITypeEditor))]
        public DirectoryInfo VlcLibDirectory { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsPlaying
        {
            get
            {
                return myVlcMediaPlayer.IsPlaying();
            }
        }

        public event EventHandler<VlcLibDirectoryNeededEventArgs> VlcLibDirectoryNeeded;

        public VlcRincewindControl()
        {
        }

        protected override void Dispose(bool disposing)
        {
            lock(myEventSyncLocker)
            {
                if (myVlcMediaPlayer != null)
                {
                    UnregisterEvents();
                    myVlcMediaPlayer.Dispose();
                    base.Dispose(disposing);
                    GC.SuppressFinalize(this);
                }
            }
        }

        public void BeginInit()
        {

        }

        public void EndInit()
        {
            if (DesignMode || myVlcMediaPlayer != null)
                return;
            if (VlcLibDirectory == null && (VlcLibDirectory = OnVlcLibDirectoryNeeded()) == null)
            {
                throw new Exception("'VlcLibDirectory' must be set.");
            }
            myVlcMediaPlayer = new VlcMediaPlayer(VlcLibDirectory);
            myVlcMediaPlayer.VideoHostHandle = Handle;
            RegisterEvents();
        }

        public DirectoryInfo OnVlcLibDirectoryNeeded()
        {
            var del = VlcLibDirectoryNeeded;
            if (del != null)
            {
                var args = new VlcLibDirectoryNeededEventArgs();
                del(this, args);
                return args.VlcLibDirectory;
            }
            return null;
        }

        public void Play()
        {
            EndInit();
            myVlcMediaPlayer.Play();
        }

#if NET20
        public void Play(FileInfo file)
        {
            EndInit();
            Play(file, null);
        }

        public void Play(FileInfo file, string[] options)
#else
        public void Play(FileInfo file, params string[] options)
#endif
        {
            EndInit();
            myVlcMediaPlayer.SetMedia(file, options);
            Play();
        }

#if NET20
        public void Play(Uri uri)
        {
            EndInit();
            Play(uri, null);
        }

        public void Play(Uri uri, string[] options)
#else
        public void Play(Uri uri, params string[] options)
#endif
        {
            EndInit();
            myVlcMediaPlayer.SetMedia(uri, options);
            Play();
        }

        public void Stop()
        {
            EndInit();
            myVlcMediaPlayer.Stop();
        }
    }
}