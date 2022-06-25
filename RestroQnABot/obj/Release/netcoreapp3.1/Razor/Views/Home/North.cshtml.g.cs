#pragma checksum "C:\Users\Manish\Source\Repos\Bot2022\RestroQnABot\Views\Home\North.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "171fc38e4701bab8810fe74094eb273658c54fd7"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_North), @"mvc.1.0.view", @"/Views/Home/North.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"171fc38e4701bab8810fe74094eb273658c54fd7", @"/Views/Home/North.cshtml")]
    #nullable restore
    public class Views_Home_North : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    #nullable disable
    {
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("<!DOCTYPE html>\r\n<html lang=\"en\">\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("head", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "171fc38e4701bab8810fe74094eb273658c54fd72759", async() => {
                WriteLiteral("\r\n    <meta charset=\"utf-8\">\r\n    <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\r\n    <title>WebChat</title>\r\n    <meta name=\"description\"");
                BeginWriteAttribute("content", " content=\"", 186, "\"", 196, 0);
                EndWriteAttribute();
                WriteLiteral(@">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
    <style>
        html,
        body {
            height: 100%;
            width: 100%;
            margin: 0;
        }

        #webchat {
            height: 100%;
            width: 90%;
        }

            #webchat > * {
                height: 100%;
                width: 100%;
            }
    </style>
");
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "171fc38e4701bab8810fe74094eb273658c54fd74461", async() => {
                WriteLiteral(@"
    <!--<iframe src='https://webchat.botframework.com/embed/RBot2022Bot?s=9JynVTUo6As.OZDLJMTWlJTLyC6xGxwbfSOHNYqXFputIfwqdkqFBBQ' style='min-width: 400px; width: 100%; min-height: 500px;'></iframe>
    <div id=""webchat"" role=""main""></div>
    <script type=""text/javascript""
            src=""https://unpkg.com/markdown-it/dist/markdown-it.min.js""></script>
    <script src=""https://cdn.botframework.com/botframework-webchat/master/webchat.js""></script>-->
    <div id=""webchat"" role=""main""></div>
    <script type=""text/javascript""
            src=""https://unpkg.com/markdown-it/dist/markdown-it.min.js""></script>
    <script src=""https://cdn.botframework.com/botframework-webchat/latest/webchat.js""></script>
    <script>
       
        (async function () {

            let conversationId;
            let token;
            let directLine;

            conversationId = window.sessionStorage[""conversationId""];
            token = window.sessionStorage[""token""];

            if (conversationId != ");
                WriteLiteral(@"undefined) {

                const res = await fetch(`https://directline.botframework.com/v3/directline/conversations/${conversationId}`, {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${token}`,
                    },
                });

                let { conversation_Id } = await res.json();
                sessionStorage['conversationId'] = conversation_Id;
            }
            else {

                var myHeaders = new Headers();
                myHeaders.append(""Content-Type"", ""application/json"");
                myHeaders.append(""Authorization"", ""Bearer Y5Q8eF-nQpg.eAaDdLhXgVaQIxspU1x3KemsfdKFbTR7x2IcnKUL0s8"");

                var raw = JSON.stringify({
                    ""user"": {
                        ""id"": ""dl_north"",
                        ""name"": ""north""
                    },
                    ""trustedOrigins"": [
                        ""string""
                    ]
                }");
                WriteLiteral(@");

                var requestOptions = {
                    method: 'POST',
                    headers: myHeaders,
                    body: raw,
                    redirect: 'follow'
                };

                const res = await fetch('https://directline.botframework.com/v3/directline/tokens/generate', requestOptions);
                const { token: directLineToken, conversation_Id } = await res.json();

                token = directLineToken;
                sessionStorage.conversationId = conversation_Id;
                sessionStorage.token = token;
            }

            const store = window.WebChat.createStore({}, ({ dispatch }) => next => action => {
                if (action.type === 'DIRECT_LINE/CONNECT_FULFILLED') {
                    // When we receive DIRECT_LINE/CONNECT_FULFILLED action, we will send an event activity using WEB_CHAT/SEND_EVENT
                    dispatch({
                        type: 'WEB_CHAT/SEND_EVENT',
                        paylo");
                WriteLiteral(@"ad: {
                            name: 'webchat/join',
                            value: '{""data"": [""NorthIndianCalories.xlsx"",""DhabaExpress.xlsx""]}'
                        }
                    });
                }
                return next(action);
            });

            directLine =  window.WebChat.createDirectLine({
                token: token
            });

            window.WebChat.renderWebChat(
                {
                    directLine,
                    sendTypingIndicator: true,
                    userID: 'dl_north',
                    username: ""north"",
                    locale: 'en-US',
                    botAvatarInitials: 'WC',
                    userAvatarInitials: 'WW',
                    store
                },
                document.getElementById('webchat')
            );


            //directLine.postActivity({
            //    from: { id: 'dl_north', name: 'north' }, // required (from.name is optional)
            //    ty");
                WriteLiteral(@"pe: 'conversationUpdate',
            //    name : 'knowleageBaseSource',
            //    text: '{""data"": [""NorthIndianCalories.xlsx"",""DhabaExpress.xlsx""]}'
            //}).subscribe(
            //    id => console.log(""Posted activity, assigned ID "", id),
            //    error => console.log(""Error posting activity"", error)
            //);

        })().catch(err => console.error(err));
    </script>
");
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n</html>");
        }
        #pragma warning restore 1998
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
