using Microsoft.Owin;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Collections.Generic;
using System.Security.Claims;
using System.IdentityModel.Services;
using System;
namespace ConsoleApplication1
{
    public class OwinContextMiddleware : OwinMiddleware
    {
        public OwinContextMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            int test = 1;
            try
            {
                OwinCallContext.Set(context);
                if (test == 1)
                {
                    //validate token
                    const string sec = "ProEMLh5e_qnzdNUQrqdHPgp";
                    const string sec1 = "ProEMLh5e_qnzdNU";
                    var securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(sec));
                    var securityKey1 = new SymmetricSecurityKey(Encoding.Default.GetBytes(sec1));

                    // This is the input JWT which we want to validate.
                    string tokenString = "eyJhbGciOiJBMTI4S1ciLCJlbmMiOiJBMTI4Q0JDLUhTMjU2IiwidHlwIjoiSldUIn0.tRxgOq6KhMF3iMMLvWA_nIIx3dk3vfUT_JqdcMBMsjEtsnlJPre2dg.pnsytilmOxuHyny47unEHg.7YAE4f0d9vg1W22NFn-yIo2VMKN6ICMyk0OUr0yCMPmgsCvpSxP-v4k_N61A_prrsPmrfxwi-MPgiYTRPbnh4-jUYYvAtqr2FctzOJJPR0Pt6MRtNh1gXzM1bI67HtDxcbxBg2K3QPj_gpnQUUON06kE9WRRnBjbM4AeAbNr12lKvnmnlC9Grurk0QTSzLwaolf3buXTmwl6kSka_WW66sHZL0RahT7pjT28r0GD5I9veZC5ZC0lzSFaprOjsy6FiZ5YpiV0L5aNZX7KhF1N8fnjuuhNKEugHpdW3cA4f-ehTu0EjiErESly7-TBcVHpjN3yevARV8iSkZY3GywZmw.MOaTPpkobSeKi0uvbO_mEw";

                    // If we retrieve the token without decrypting the claims, we won't get any claims
                    // DO not use this jwt variable
                    var jwt = new JwtSecurityToken(tokenString);

                    // Verification
                    var tokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidAudiences = new string[]
                        {
        "536481524875-glk7nibpj1q9c4184d4n3gittrt8q3mn.apps.googleusercontent.com"
                        },
                        ValidIssuers = new string[]
                        {
        "https://accounts.google.com"
                        },
                        IssuerSigningKey = securityKey,
                        // This is the decryption key
                        TokenDecryptionKey = securityKey1
                    };

                    SecurityToken validatedToken;
                    var handler = new JwtSecurityTokenHandler();

                    handler.ValidateToken(tokenString, tokenValidationParameters, out validatedToken);
                    //end
                    await Next.Invoke(context);
                }

                else
                {

                    // create Jwt token
                    var plainTextSecurityKey = "This is my shared, not so secret, secret!";
                    var signingKey = new InMemorySymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(plainTextSecurityKey));
                    var signingCredentials = new SigningCredentials(signingKey,
                        SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);
                    //end
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.ContentType = "application/json";
                    context.Response.Write(jwt.RawEncryptedKey.ToString());
                }

            }
            catch (Exception ex) {
                throw ex;
            }
            finally
            {
                OwinCallContext.Remove(context);
            }
        }
    }

    /// <summary>
    /// Helper class for setting and accessing the current <see cref="IOwinContext"/>
    /// </summary>
    public class OwinCallContext
    {
        private const string OwinContextKey = "owin.IOwinContext";

        public static IOwinContext Current
        {
            get { return (IOwinContext)CallContext.LogicalGetData(OwinContextKey); }
        }

        public static void Set(IOwinContext context)
        {
            CallContext.LogicalSetData(OwinContextKey, context);
        }

        public static void Remove(IOwinContext context)
        {
            CallContext.FreeNamedDataSlot(OwinContextKey);
        }
    }
}
