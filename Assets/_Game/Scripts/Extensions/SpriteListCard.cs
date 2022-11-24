/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Cards
{
    [CreateAssetMenu(fileName ="Card Sprite List", menuName ="ScriptableObjects/Lists/Sprite List/Card")]
    public class SpriteListCard : ScriptableObject
    {
        #region Properties
        [SerializeField] private Sprite _soulValueSprite = null;
        public Sprite SoulValueSprite => _soulValueSprite;
        [SerializeField] private RarityIcons _rarityIcons = new RarityIcons();
        public RarityIcons RaritySprites => _rarityIcons;
        #endregion

        [Serializable]
        public class RarityIcons
        {
            [SerializeField] private Sprite _raritySpriteCommon = null;
            public Sprite Common => _raritySpriteCommon;
            [SerializeField] private Sprite _raritySpriteRare = null;
            public Sprite Rare => _raritySpriteRare;
            [SerializeField] private Sprite _raritySpriteVeryRare = null;
            public Sprite VeryRare => _raritySpriteVeryRare;
            [SerializeField] private Sprite _raritySpriteSpecial = null;
            public Sprite Special => _raritySpriteSpecial;
        }
    }
}
