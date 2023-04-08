//
//  S3Client.swift
//  ios
//
//  Created by hzp on 2023/4/8.
//  Copyright © 2023 hzp. All rights reserved.
//

import Foundation
import AWSS3
import AWSCore

class CucClient:ObservableObject{
    var s3: AWSS3!
    var bucktName: String!
    
    init() {
        let credentialsProvider = AWSStaticCredentialsProvider(accessKey: "<your ak>", secretKey: "<your sk>")
        let configuration = AWSServiceConfiguration(region: .USWest1 ,endpoint: AWSEndpoint(urlString: "<bucket endpoint>"), credentialsProvider: credentialsProvider)
        
        
        AWSServiceManager.default().defaultServiceConfiguration=configuration
        self.bucktName = "<your bucket>"
        self.s3 = AWSS3.default()
    }
    
    
    func ListBucket(){
        print("# Start List Bucket:")
        let req: AWSS3ListBucketInventoryConfigurationsRequest = AWSS3ListBucketInventoryConfigurationsRequest()
        self.s3.listBuckets(req).continueWith(block: {(task) ->AnyObject? in
            for o in (task.result?.buckets)!{
                print(o.name ?? "null")
            }
            return nil
        })
        return
    }
    
    func ListObjects(){
        print("# Start List Bucket Objet:")
        let req: AWSS3ListObjectsRequest = AWSS3ListObjectsRequest()
        req.bucket = self.bucktName
        self.s3.listObjects(req).continueWith(block: {(task)->AnyObject? in
            for o in (task.result?.contents)!{
                print("object name: \(String(o.key!))")
            }
            return nil
        })
        return
    }
    
    func PutObject(){
        print("# Start Put Object")
        let req = AWSS3PutObjectRequest()!
        req.bucket = self.bucktName
        req.key = "Client.swift"
        
        let data = try! String(contentsOfFile:"/Users/tharp/Desktop/CucObsSample/ios/ios/Client.swift", encoding: String.Encoding.utf8)
        req.body = data
        req.contentLength = NSNumber(value: data.count)
        let semaphore = DispatchSemaphore(value: 0)
        
        s3.putObject(req) { result, error in
            defer {
                semaphore.signal()
            }
            if let e = error {
               print(e)
                return
            }
            guard result != nil else {
                print ("Result unexpectedly nil")
                return
            }
            print("上传成功！")
        }
        semaphore.wait()
        return
    }
    
    
    func GetObject(){
        print("# Start Get Object.")
        let req = AWSS3GetObjectRequest()!
        req.bucket = self.bucktName
        req.key = "Client.swift"
        
        self.s3.getObject(req){result, error in
            if let e = error{
                print(e)
            }else{
                print("读取文件大小：\(String(describing: result?.contentLength))")
            }
        }
    }
    
    func DeletObject(){
        print("# Start Delete Object.")
        let req = AWSS3DeleteObjectRequest()!
        req.bucket = self.bucktName
        req.key = "Client.swift"
        
        self.s3.deleteObject(req){result, error in
            if let e = error{
                print(e)
            }else{
                print("success")
            }
        }
    }
}
