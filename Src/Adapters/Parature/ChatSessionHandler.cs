/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *

********************************************************/
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Csr;
using System;
using System.Collections.Generic;

namespace Microsoft.USD.ComponentLibrary.Parature
{
    public class ChatSessionHandler : DynamicsBaseHostedControl
    {
        Dictionary<Guid, string> chatSessions;

        public ChatSessionHandler(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {
            chatSessions = new Dictionary<Guid, string>();
        }

        bool addToNextSession = false;
        ChatSession nextChat;
        protected override void DoAction(RequestActionEventArgs args)
        {
            if (args.Action.Equals("AddChat", StringComparison.OrdinalIgnoreCase))
            {
                List<KeyValuePair<string, string>> parameterlist = Utility.SplitLines(args.Data, CurrentContext, localSession);
                string chatId = Utility.GetAndRemoveParameter(parameterlist, "chatId");
                string sessionId = Utility.GetAndRemoveParameter(parameterlist, "sessionId");
                string nextSession = Utility.GetAndRemoveParameter(parameterlist, "addToNextSession");
                string currentSession = Utility.GetAndRemoveParameter(parameterlist, "addToCurrentSession");

                if (string.IsNullOrEmpty(chatId))
                    return;

                Guid session = Guid.Empty;
                if (!string.IsNullOrEmpty(sessionId))
                {
                    session = Guid.Parse(sessionId);
                    if (chatSessions.ContainsKey(session))
                        throw new Exception("Session already added");
                    chatSessions.Add(session, chatId);
                }
                else if (!string.IsNullOrEmpty(currentSession) && currentSession.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    session = localSessionManager.ActiveSession.SessionId;
                    if (chatSessions.ContainsKey(session))
                        throw new Exception("Session already added");
                    chatSessions.Add(session, chatId);
                }
                else if (!string.IsNullOrEmpty(nextSession) && nextSession.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    //differ this chat until a new session was added
                    addToNextSession = true;
                    nextChat.ChatID = chatId;
                }

                if (session != Guid.Empty)
                {
                    var parameters = new Dictionary<string, string>();
                    parameters.Add("SessionID", session.ToString());
                    parameters.Add("ChatID", chatSessions[session]);
                    FireEvent("ChatAdded", parameters);
                }
            }
            else if (args.Action.Equals("RemoveChat", StringComparison.OrdinalIgnoreCase))
            {
                List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
                string chatId = Utility.GetAndRemoveParameter(parameters, "chatId");
                string sessionId = Utility.GetAndRemoveParameter(parameters, "sessionId");

                Guid session = Guid.Parse(sessionId);

                //switch to the chat & end & switch back
                if (chatSessions.ContainsKey(session))
                {
                    chatSessions.Remove(session);

                    var eventParams = new Dictionary<string, string>();
                    eventParams.Add("SessionID", session.ToString());
                    eventParams.Add("ChatID", chatSessions[session]);

                    FireEvent("ChatRemoved", eventParams);
                }
            }
            else if (args.Action.Equals("SessionIdFromChatId", StringComparison.OrdinalIgnoreCase))
            {
                List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
                string chatId = Utility.GetAndRemoveParameter(parameters, "chatId");

                foreach (Guid key in chatSessions.Keys)
                {
                    if (chatSessions[key] == chatId)
                    {
                        args.ActionReturnValue = key.ToString();
                    }
                }
            }
        }

        public override void SessionChange(bool activate, Guid sessionID)
        {
            base.SessionChange(activate, sessionID);

            if (activate & chatSessions.ContainsKey(sessionID))
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("SessionID", sessionID.ToString());
                parameters.Add("ChatID", chatSessions[sessionID]);
                FireEvent("SwitchChat", parameters);
            }
            else if (activate)
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("SessionID", sessionID.ToString());
                FireEvent("HideChat", parameters);
            }
        }

        protected override void SessionCreatedEvent(Session session)
        {
            base.SessionCreatedEvent(session);

            if (addToNextSession)
            {
                nextChat.SessionID = session.SessionId;
                addToNextSession = false;
                chatSessions.Add(nextChat.SessionID, nextChat.ChatID);

                var parameters = new Dictionary<string, string>();
                parameters.Add("SessionID", session.SessionId.ToString());
                parameters.Add("ChatID", chatSessions[session.SessionId]);
                FireEvent("ChatAdded", parameters);
            }
            else
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("SessionID", session.SessionId.ToString());
                FireEvent("HideChat", parameters);
            }
        }

        protected override bool SessionCloseEvent(Session session)
        {
            if (!session.Global
                && chatSessions.ContainsKey(session.SessionId))
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("SessionID", session.SessionId.ToString());
                parameters.Add("ChatID", chatSessions[session.SessionId]);

                chatSessions.Remove(session.SessionId);

                FireEvent("ChatRemoved", parameters);
            }

            return base.SessionCloseEvent(session);
        }
    }

    struct ChatSession
    {
        public string ChatID;
        public Guid SessionID;
    }
}
