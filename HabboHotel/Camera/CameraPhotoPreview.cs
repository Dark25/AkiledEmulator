using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akiled.HabboHotel.Camera
{
    public class CameraPhotoPreview
    {
        private int _photoId;
        private int _creatorId;
        private long _createdAt;

        public int Id
        {
            get
            {
                return this._photoId;
            }
        }

        public int CreatorId
        {
            get
            {
                return this._creatorId;
            }
        }

        public long CreatedAt
        {
            get
            {
                return this._createdAt;
            }
        }

        public CameraPhotoPreview(int photoId, int creatorId, long createdAt)
        {
            this._photoId = photoId;
            this._creatorId = creatorId;
            this._createdAt = createdAt;
        }
    }
}