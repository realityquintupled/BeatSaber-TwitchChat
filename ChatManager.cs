﻿using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

namespace TwitchIRC
{
    class ChatManager : MonoBehaviour
    {
        private List<string> chat;
        private TextMeshPro tmp;
        private string opacity;
        private Vector3 originalPosition;
        private Regex stopUsingTagsYouAsshats = new Regex("(?=<)(.*?)(?<=>)", RegexOptions.Compiled);
        public void Start()
        {
            chat = new List<string>();
            opacity = Plugin.config.opacity.ToString("X2");
            tmp = gameObject.AddComponent<TextMeshPro>();
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.pivot = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(1, 2);
            tmp.fontSize = .35f;
            tmp.alignment = TextAlignmentOptions.BottomLeft;
            originalPosition = transform.localPosition;
        }
        public void OnMessageReceived(ChatMessage chatMessage)
        {
            if (Plugin.config.ignoreMessagesWithPrefix != string.Empty && chatMessage.message.StartsWith(Plugin.config.ignoreMessagesWithPrefix))
                return;
            if (Plugin.config.disableChatTags)
                chatMessage.message = stopUsingTagsYouAsshats.Replace(chatMessage.message, string.Empty);
            string message = chatMessage.message.ToLower().Contains(Plugin.config.username.ToLower()) ? $"<color={Plugin.config.taggedMessagesColor + opacity}>{chatMessage.message}</color>" : chatMessage.message;
            string color = (Plugin.config.overrideNameColor ? Plugin.config.customNameColor : chatMessage.color) + opacity;
            chat.Add($"<color={color}>{chatMessage.username}</color>: {message}");
            if (chat.Count > Plugin.config.maxChatMessages)
                chat.RemoveRange(0, chat.Count - Plugin.config.maxChatMessages);
            UpdateChatDisplay();
        }
        private void UpdateChatDisplay()
        {
            string chatString = $"<color={Plugin.config.chatMessagesColor+opacity}>";
            chat.ForEach((message) =>
            {
                chatString += message + "\n";
            });
            chatString += "</color>";
            tmp.text = chatString;
            tmp.ForceMeshUpdate();
        }
        public void Update() {
            if (!Plugin.config.chatTwitch)
                return;
            transform.localPosition = originalPosition + new Vector3(Random.Range(-Plugin.config.chatTwitchOffset, Plugin.config.chatTwitchOffset), Random.Range(-Plugin.config.chatTwitchOffset, Plugin.config.chatTwitchOffset), Random.Range(-Plugin.config.chatTwitchOffset, Plugin.config.chatTwitchOffset));
        }
    }
}
