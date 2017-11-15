using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace softblocks.Services
{
    public class AmazonService
    {
        private readonly string accessKeyId;
        private readonly string secretAccessKeyId;
        private readonly string bucketName;
        private Amazon.RegionEndpoint Region = Amazon.RegionEndpoint.USEast1;

        public AmazonService()
        {
            accessKeyId = ConfigurationManager.AppSettings["AWSAccessKey"].ToString();
            secretAccessKeyId = ConfigurationManager.AppSettings["AWSSecretKey"].ToString();
            bucketName = ConfigurationManager.AppSettings["AWSBucketName"].ToString();
        }

        public void S3Upload(string key, string mimeType, MemoryStream memoryStream, bool isPublic = false)
        {
            try
            {
                using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(accessKeyId, secretAccessKeyId, Region))
                {
                    var request = new PutObjectRequest();
                    request.BucketName = bucketName;
                    request.Key = key;
                    request.ContentType = mimeType;
                    request.InputStream = memoryStream;

                    if (isPublic)
                    {
                        request.CannedACL = S3CannedACL.PublicRead;
                    }

                    var response = client.PutObject(request);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public byte[] ReteiveFile(string key)
        {
            using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(accessKeyId, secretAccessKeyId, Region))
            {
                var request = new GetObjectRequest();
                request.BucketName = bucketName;
                request.Key = key;

                try
                {
                    using (var response = client.GetObject(request))
                    {
                        using (Stream s = response.ResponseStream)
                        {
                            var status = response.HttpStatusCode;
                            MemoryStream memoryStream = new MemoryStream();
                            s.CopyTo(memoryStream);
                            var bytes = memoryStream.ToArray();
                            memoryStream.Close();
                            return bytes;
                        }
                    }
                }
                catch (Amazon.S3.AmazonS3Exception ex)
                {
                    if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        Byte[] array = new Byte[0];
                        Array.Clear(array, 0, array.Length);
                        return array;
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
        }
    }
}