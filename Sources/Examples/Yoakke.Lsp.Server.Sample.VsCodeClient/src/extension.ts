import * as path from 'path';
import { workspace, ExtensionContext } from 'vscode';
import {
	LanguageClient,
	LanguageClientOptions,
	ServerOptions,
	TransportKind,
} from 'vscode-languageclient/node';

let client: LanguageClient;

export function activate(context: ExtensionContext) {
	// Path for the server
	let serverPath = context.asAbsolutePath(path.join('out', 'Yoakke.Lsp.Server.Sample.exe'));

	// Server options
	let serverOptions: ServerOptions = {
		command: serverPath,
		transport: TransportKind.stdio,
	};

	// Client options
	let clientOptions: LanguageClientOptions = {
		documentSelector: [{ scheme: 'file', language: 'plaintext' }],
	};

	client = new LanguageClient(
		"sampleLanguageServer",
		'Sample Language Server',
		serverOptions,
		clientOptions
	);

	// Start the client, which also starts the server
	client.start();
}

export function deactivate() {
	if (!client) return undefined;
	return client.stop();
}
