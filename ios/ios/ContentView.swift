//
//  ContentView.swift
//  ios
//
//  Created by hzp on 2023/4/8.
//  Copyright © 2023 hzp. All rights reserved.
//

import SwiftUI


struct ContentView: View {
    @ObservedObject var client:CucClient = CucClient()

    var body: some View {
        ScrollView(.vertical,showsIndicators: true){
            VStack{
                Button(action: {self.client.ListBucket()}){Text("List Bucket")}.padding()
                Button(action: {self.client.ListObjects()}){Text("List Bucket Object")}.padding()
                Button(action: {self.client.PutObject()}){Text("Put Object")}.padding()
                Button(action: {self.client.GetObject()}){Text("Get Object")}.padding()
                Button(action:{self.client.DeletObject()}){Text("Delete Object")}
            }
        }

    }
}

struct ContentView_Previews: PreviewProvider {

    static var previews: some View {
        ContentView()
    }
}
