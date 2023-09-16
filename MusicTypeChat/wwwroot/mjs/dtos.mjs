/* Options:
Date: 2023-09-17 01:42:17
Version: 6.101
Tip: To override a DTO option, remove "//" prefix before updating
BaseUrl: https://localhost:5001

//AddServiceStackTypes: True
//AddDocAnnotations: True
//AddDescriptionAsComments: True
//IncludeTypes: 
//ExcludeTypes: 
//DefaultImports: 
*/

"use strict";
export class Recording {
    /** @param {{id?:number,feature?:string,provider?:string,path?:string,transcript?:string,transcriptConfidence?:number,transcriptResponse?:string,createdDate?:string,transcribeStart?:string,transcribeEnd?:string,transcribeDurationMs?:number,durationMs?:number,ipAddress?:string,error?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {string} */
    feature;
    /** @type {string} */
    provider;
    /** @type {string} */
    path;
    /** @type {?string} */
    transcript;
    /** @type {?number} */
    transcriptConfidence;
    /** @type {?string} */
    transcriptResponse;
    /** @type {string} */
    createdDate;
    /** @type {?string} */
    transcribeStart;
    /** @type {?string} */
    transcribeEnd;
    /** @type {?number} */
    transcribeDurationMs;
    /** @type {?number} */
    durationMs;
    /** @type {?string} */
    ipAddress;
    /** @type {?string} */
    error;
}
/** @typedef {'Json'|'Program'} */
export var TypeChatTranslator;
(function (TypeChatTranslator) {
    TypeChatTranslator["Json"] = "Json"
    TypeChatTranslator["Program"] = "Program"
})(TypeChatTranslator || (TypeChatTranslator = {}));
export class Chat {
    /** @param {{id?:number,feature?:string,provider?:string,request?:string,prompt?:string,schema?:string,chatResponse?:string,createdDate?:string,chatStart?:string,chatEnd?:string,chatDurationMs?:number,ipAddress?:string,error?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {string} */
    feature;
    /** @type {string} */
    provider;
    /** @type {string} */
    request;
    /** @type {string} */
    prompt;
    /** @type {string} */
    schema;
    /** @type {?string} */
    chatResponse;
    /** @type {string} */
    createdDate;
    /** @type {?string} */
    chatStart;
    /** @type {?string} */
    chatEnd;
    /** @type {?number} */
    chatDurationMs;
    /** @type {?string} */
    ipAddress;
    /** @type {?string} */
    error;
}
export class QueryBase {
    /** @param {{skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {?number} */
    skip;
    /** @type {?number} */
    take;
    /** @type {string} */
    orderBy;
    /** @type {string} */
    orderByDesc;
    /** @type {string} */
    include;
    /** @type {string} */
    fields;
    /** @type {{ [index: string]: string; }} */
    meta;
}
/** @typedef T {any} */
export class QueryDb extends QueryBase {
    /** @param {{skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
}
export class ResponseError {
    /** @param {{errorCode?:string,fieldName?:string,message?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    errorCode;
    /** @type {string} */
    fieldName;
    /** @type {string} */
    message;
    /** @type {{ [index: string]: string; }} */
    meta;
}
export class ResponseStatus {
    /** @param {{errorCode?:string,message?:string,stackTrace?:string,errors?:ResponseError[],meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    errorCode;
    /** @type {string} */
    message;
    /** @type {string} */
    stackTrace;
    /** @type {ResponseError[]} */
    errors;
    /** @type {{ [index: string]: string; }} */
    meta;
}
export class TypeChatStep {
    /** @param {{_func?:string,_args?:Object[]}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    _func;
    /** @type {Object[]} */
    _args;
}
export class PageStats {
    /** @param {{label?:string,total?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    label;
    /** @type {number} */
    total;
}
export class StringsResponse {
    /** @param {{results?:string[],meta?:{ [index: string]: string; },responseStatus?:ResponseStatus}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string[]} */
    results;
    /** @type {{ [index: string]: string; }} */
    meta;
    /** @type {ResponseStatus} */
    responseStatus;
}
export class CreateSpotifyChatResponse {
    /** @param {{stepResults?:Object[],result?:Object,_steps?:TypeChatStep[],responseStatus?:ResponseStatus}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {Object[]} */
    stepResults;
    /** @type {?Object} */
    result;
    /** @type {TypeChatStep[]} */
    _steps;
    /** @type {ResponseStatus} */
    responseStatus;
}
export class AdminDataResponse {
    /** @param {{pageStats?:PageStats[]}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {PageStats[]} */
    pageStats;
}
export class AuthenticateResponse {
    /** @param {{userId?:string,sessionId?:string,userName?:string,displayName?:string,referrerUrl?:string,bearerToken?:string,refreshToken?:string,profileUrl?:string,roles?:string[],permissions?:string[],responseStatus?:ResponseStatus,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    userId;
    /** @type {string} */
    sessionId;
    /** @type {string} */
    userName;
    /** @type {string} */
    displayName;
    /** @type {string} */
    referrerUrl;
    /** @type {string} */
    bearerToken;
    /** @type {string} */
    refreshToken;
    /** @type {string} */
    profileUrl;
    /** @type {string[]} */
    roles;
    /** @type {string[]} */
    permissions;
    /** @type {ResponseStatus} */
    responseStatus;
    /** @type {{ [index: string]: string; }} */
    meta;
}
export class AssignRolesResponse {
    /** @param {{allRoles?:string[],allPermissions?:string[],meta?:{ [index: string]: string; },responseStatus?:ResponseStatus}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string[]} */
    allRoles;
    /** @type {string[]} */
    allPermissions;
    /** @type {{ [index: string]: string; }} */
    meta;
    /** @type {ResponseStatus} */
    responseStatus;
}
export class UnAssignRolesResponse {
    /** @param {{allRoles?:string[],allPermissions?:string[],meta?:{ [index: string]: string; },responseStatus?:ResponseStatus}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string[]} */
    allRoles;
    /** @type {string[]} */
    allPermissions;
    /** @type {{ [index: string]: string; }} */
    meta;
    /** @type {ResponseStatus} */
    responseStatus;
}
export class RegisterResponse {
    /** @param {{userId?:string,sessionId?:string,userName?:string,referrerUrl?:string,bearerToken?:string,refreshToken?:string,roles?:string[],permissions?:string[],responseStatus?:ResponseStatus,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    userId;
    /** @type {string} */
    sessionId;
    /** @type {string} */
    userName;
    /** @type {string} */
    referrerUrl;
    /** @type {string} */
    bearerToken;
    /** @type {string} */
    refreshToken;
    /** @type {string[]} */
    roles;
    /** @type {string[]} */
    permissions;
    /** @type {ResponseStatus} */
    responseStatus;
    /** @type {{ [index: string]: string; }} */
    meta;
}
/** @typedef T {any} */
export class QueryResponse {
    /** @param {{offset?:number,total?:number,results?:T[],meta?:{ [index: string]: string; },responseStatus?:ResponseStatus}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    offset;
    /** @type {number} */
    total;
    /** @type {T[]} */
    results;
    /** @type {{ [index: string]: string; }} */
    meta;
    /** @type {ResponseStatus} */
    responseStatus;
}
export class GetSchema {
    /** @param {{feature?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    feature;
    getTypeName() { return 'GetSchema' }
    getMethod() { return 'POST' }
    createResponse() { return '' }
}
export class GetPrompt {
    /** @param {{feature?:string,userMessage?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    feature;
    /** @type {string} */
    userMessage;
    getTypeName() { return 'GetPrompt' }
    getMethod() { return 'POST' }
    createResponse() { return '' }
}
export class GetPhrases {
    /** @param {{feature?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    feature;
    getTypeName() { return 'GetPhrases' }
    getMethod() { return 'POST' }
    createResponse() { return new StringsResponse() }
}
export class InitSpeech {
    /** @param {{feature?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    feature;
    getTypeName() { return 'InitSpeech' }
    getMethod() { return 'POST' }
    createResponse() { }
}
export class CreateRecording {
    /** @param {{feature?:string,path?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    feature;
    /** @type {string} */
    path;
    getTypeName() { return 'CreateRecording' }
    getMethod() { return 'POST' }
    createResponse() { return new Recording() }
}
export class CreateChat {
    /** @param {{feature?:string,userMessage?:string,translator?:TypeChatTranslator}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    feature;
    /** @type {string} */
    userMessage;
    /** @type {?TypeChatTranslator} */
    translator;
    getTypeName() { return 'CreateChat' }
    getMethod() { return 'POST' }
    createResponse() { return new Chat() }
}
export class CreateSpotifyChat {
    /** @param {{userMessage?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    userMessage;
    getTypeName() { return 'CreateSpotifyChat' }
    getMethod() { return 'POST' }
    createResponse() { return new CreateSpotifyChatResponse() }
}
export class AdminData {
    constructor(init) { Object.assign(this, init) }
    getTypeName() { return 'AdminData' }
    getMethod() { return 'GET' }
    createResponse() { return new AdminDataResponse() }
}
export class Authenticate {
    /** @param {{provider?:string,state?:string,oauth_token?:string,oauth_verifier?:string,userName?:string,password?:string,rememberMe?:boolean,errorView?:string,nonce?:string,uri?:string,response?:string,qop?:string,nc?:string,cnonce?:string,accessToken?:string,accessTokenSecret?:string,scope?:string,returnUrl?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { Object.assign(this, init) }
    /**
     * @type {string}
     * @description AuthProvider, e.g. credentials */
    provider;
    /** @type {string} */
    state;
    /** @type {string} */
    oauth_token;
    /** @type {string} */
    oauth_verifier;
    /** @type {string} */
    userName;
    /** @type {string} */
    password;
    /** @type {?boolean} */
    rememberMe;
    /** @type {string} */
    errorView;
    /** @type {string} */
    nonce;
    /** @type {string} */
    uri;
    /** @type {string} */
    response;
    /** @type {string} */
    qop;
    /** @type {string} */
    nc;
    /** @type {string} */
    cnonce;
    /** @type {string} */
    accessToken;
    /** @type {string} */
    accessTokenSecret;
    /** @type {string} */
    scope;
    /** @type {string} */
    returnUrl;
    /** @type {{ [index: string]: string; }} */
    meta;
    getTypeName() { return 'Authenticate' }
    getMethod() { return 'POST' }
    createResponse() { return new AuthenticateResponse() }
}
export class AssignRoles {
    /** @param {{userName?:string,permissions?:string[],roles?:string[],meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    userName;
    /** @type {string[]} */
    permissions;
    /** @type {string[]} */
    roles;
    /** @type {{ [index: string]: string; }} */
    meta;
    getTypeName() { return 'AssignRoles' }
    getMethod() { return 'POST' }
    createResponse() { return new AssignRolesResponse() }
}
export class UnAssignRoles {
    /** @param {{userName?:string,permissions?:string[],roles?:string[],meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    userName;
    /** @type {string[]} */
    permissions;
    /** @type {string[]} */
    roles;
    /** @type {{ [index: string]: string; }} */
    meta;
    getTypeName() { return 'UnAssignRoles' }
    getMethod() { return 'POST' }
    createResponse() { return new UnAssignRolesResponse() }
}
export class Register {
    /** @param {{userName?:string,firstName?:string,lastName?:string,displayName?:string,email?:string,password?:string,confirmPassword?:string,autoLogin?:boolean,errorView?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    userName;
    /** @type {string} */
    firstName;
    /** @type {string} */
    lastName;
    /** @type {string} */
    displayName;
    /** @type {string} */
    email;
    /** @type {string} */
    password;
    /** @type {string} */
    confirmPassword;
    /** @type {?boolean} */
    autoLogin;
    /** @type {string} */
    errorView;
    /** @type {{ [index: string]: string; }} */
    meta;
    getTypeName() { return 'Register' }
    getMethod() { return 'POST' }
    createResponse() { return new RegisterResponse() }
}
export class QueryRecordings extends QueryDb {
    /** @param {{skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    getTypeName() { return 'QueryRecordings' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryChats extends QueryDb {
    /** @param {{skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    getTypeName() { return 'QueryChats' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}

