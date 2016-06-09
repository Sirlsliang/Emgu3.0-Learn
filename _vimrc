set nocompatible
filetype off
behave mswin

syntax on
colorscheme desert "��ʾ��ɫ����
set guioptions -=T "���ع�����
set guioptions -=m "���ز˵���

"�Զ��л���ǰĿ¼Ϊ��ǰ�ļ�����Ŀ¼
set bsdir=buffer
set autochdir 

"��������
"set enc=utf-8
"set fencs=utf-8,ucs-bom,shift-jis,gb18030,gbk,gb2312,cp936

"��������
"set langmenu-zh_CN.UTF-8
"language message zh_CN.UTF-8

set nu		"��ʾ�к�
set nobackup "��ֹ������ʱ�ļ�
set ignorecase "���Դ�Сд

set incsearch "�������ַ�����
set hlsearch "����ʱ������ʾ���ҵ����ı�

set ruler "��ʾ��������
"set cursorline "��������

"�Զ�����
set cindent 
set backspace=indent,eol,start
"Tab���Ŀ��
set tabstop=2
set shiftwidth=2

"�༭vimrc֮�����¼���
autocmd! bufwritepost _vimrc source $VIM/_vimrc

set noerrorbells "�رմ�����Ϣ����
set novisualbell "�ر�ʹ�ÿ�������������
set autoread "���ⲿ�ļ����޸�ʱ���Զ����¶�ȡ
"�﷨����
"syntax enable
"syntax on

"set shell=git
winpos 150 50 "��������λ��
set lines=40 columns=130 "���ô��ڴ�С

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
"filetype Plugin indent on "���������ļ����͵�������ϵ

call vundle#end()
filetype plugin indent on
set laststatus=2
set t_Co=256
let g:Powerline_symols='fancy'
"����NerdTree
map<F2> :NERDTreeToggle<CR>

"����vim��������ֺ�
set guifont=Consolas:h14

set smarttab
set mouse=a
