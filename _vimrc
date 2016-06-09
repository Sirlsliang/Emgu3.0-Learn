set nocompatible
filetype off
behave mswin

syntax on
colorscheme desert "显示配色方案
set guioptions -=T "隐藏工具栏
set guioptions -=m "隐藏菜单栏

"自动切换当前目录为当前文件所在目录
set bsdir=buffer
set autochdir 

"编码设置
"set enc=utf-8
"set fencs=utf-8,ucs-bom,shift-jis,gb18030,gbk,gb2312,cp936

"语言设置
"set langmenu-zh_CN.UTF-8
"language message zh_CN.UTF-8

set nu		"显示行号
set nobackup "禁止生成临时文件
set ignorecase "忽略大小写

set incsearch "搜索逐字符高亮
set hlsearch "搜索时高亮显示被找到的文本

set ruler "显示光标的坐标
"set cursorline "高亮整行

"自动缩进
set cindent 
set backspace=indent,eol,start
"Tab键的宽度
set tabstop=2
set shiftwidth=2

"编辑vimrc之后，重新加载
autocmd! bufwritepost _vimrc source $VIM/_vimrc

set noerrorbells "关闭错误信息响铃
set novisualbell "关闭使用可视响铃代替呼叫
set autoread "当外部文件被修改时，自动重新读取
"语法高亮
"syntax enable
"syntax on

"set shell=git
winpos 150 50 "设置启动位置
set lines=40 columns=130 "设置窗口大小

set shell=cmd

set rtp+=$VIM/vimfiles/bundle/vundle/
call vundle#begin('$VIM/vimfiles/bundle/')
filetype plugin indent on
Bundle 'gmarik/vundle'
Bundle 'easymotion/vim-easymotion'
Bundle 'rstacruz/sparkup',{'rtp':'VIM'}
Bundle 'L9'
"Bundle 'Valloric/YouCompleteMe'
Bundle 'scrooloose/syntastic'
Bundle 'Lokaltog/vim-powerline'
Bundle 'scrooloose/nerdtree'
"filetype Plugin indent on "激活插件与文件类型的依赖关系

call vundle#end()
filetype plugin indent on
set laststatus=2
set t_Co=256
let g:Powerline_symols='fancy'
"设置NerdTree
map<F2> :NERDTreeToggle<CR>

"设置vim的字体和字号
set guifont=Consolas:h14

set smarttab
set mouse=a
