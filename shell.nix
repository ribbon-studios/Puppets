# Shell for bootstrapping flake-enabled nix and home-manager
# Enter it through 'nix develop' or (legacy) 'nix-shell'

{ pkgs ? (import ./nixpkgs.nix) { } }: {
  default = pkgs.mkShell {
      nativeBuildInputs = with pkgs.buildPackages; [
        nodejs_18
        dotnet-sdk_8
      ];
  };
}
