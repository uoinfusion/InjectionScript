'use strict';
// The module 'vscode' contains the VS Code extensibility API
// Import the module and reference it with the alias vscode in your code below
import {  workspace, ExtensionContext } from 'vscode';
import { ServerOptions, Executable, LanguageClient, LanguageClientOptions } from 'vscode-languageclient';
import * as path from "path";
import * as fs from "fs";

const languageServerPaths = [
	"../../src/InjectionScript.Lsp.Server/bin/Debug/netcoreapp2.1/InjectionScript.Lsp.Server.dll",
	"bin/InjectionScript.Lsp.Server.dll",
];

// this method is called when your extension is activated
// your extension is activated the very first time the command is executed
export function activate(context: ExtensionContext) {
    var serverModule = null;
	for (let p of languageServerPaths) {
		p = context.asAbsolutePath(p);
		if (fs.existsSync(p)) {
			serverModule = p;
			break;
		}
    }

    if (!serverModule) { throw new URIError("Cannot find the language server module."); }
	let workPath = path.dirname(serverModule);
	console.log(`Use ${serverModule} as server module.`);
    console.log(`Work path: ${workPath}.`);

    let run: Executable = { command: "dotnet", args: [serverModule], options: {cwd: workPath} };
    let debug: Executable = { command: "dotnet", args: [serverModule, "--debug"], options: {cwd: workPath} };

    let serverOptions: ServerOptions = {
        run: run,
        debug: debug
    };
    
    // client extensions configure their server
    let clientOptions: LanguageClientOptions = {
        documentSelector: [
            { language: 'injection', scheme: 'file' },
            { language: 'injection', scheme: 'untitled' }
        ],
        synchronize: {
            configurationSection: 'injection',
            fileEvents: [
                workspace.createFileSystemWatcher('**/*.sc', false, true, false),
            ]
        }
    };

    let client = new LanguageClient('injection', serverOptions, clientOptions);
    let disposable = client.start();

    // Push the disposable to the context's subscriptions so that the
    // client can be deactivated on extension deactivation
    context.subscriptions.push(disposable);
}

// this method is called when your extension is deactivated
export function deactivate() {
}