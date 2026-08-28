{ pkgs ? import <nixpkgs> {} }:

pkgs.mkShell {
  buildInputs = [
    pkgs.icu
    pkgs.openssl
    pkgs.zlib
  ];
}
