using System.Collections.Generic;
using Spine;
using Spine.Unity;

namespace Core.Extensions
{
    public static class SpineExtensions
    {
        public static void MixSkins(this SkeletonAnimation spine, params string[] skins)
        {
            var skeletonData = spine.Skeleton.Data;
            var newSkin = new Skin(skins.GetHashCode().ToString());

            foreach (var skin in skins)
            {
                newSkin.AddSkin(skeletonData.FindSkin(skin));
            }

            spine.Skeleton.SetSkin(newSkin);
            spine.Skeleton.SetSlotsToSetupPose();
        }

        public static void ChangeAttachments(this SkeletonAnimation spine, Dictionary<string, string> slotAttachmentDictionary)
        {
            foreach (var entry in slotAttachmentDictionary)
            {
                spine.Skeleton.SetAttachment(entry.Key, entry.Value);
            }
        }
    }
}